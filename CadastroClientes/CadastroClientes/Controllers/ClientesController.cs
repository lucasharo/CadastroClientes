using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CadastroClientes.Data;
using CadastroClientes.Models;
using Microsoft.AspNetCore.Identity;
using CadastroClientes.Services;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.IdentityModel.Protocols;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CadastroClientes.Controllers
{
    public class ClientesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ClientesController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            ViewBag.Error = TempData["Error"];

            return View(await _context.Users.Where(x => x.CNPJ != null).ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,RazaoSocial,Email,CNPJ,TelefoneComercial,TelefoneCelular,CEP,Cidade,Estado")] ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Util.SomenteNumero(model.CNPJ),
                    Email = model.Email,
                    Nome = model.Nome,
                    RazaoSocial = model.RazaoSocial,
                    CEP = Util.SomenteNumero(model.CEP),
                    Cidade = model.Cidade,
                    CNPJ = Util.SomenteNumero(model.CNPJ),
                    Estado = model.Estado,
                    TelefoneCelular = Util.SomenteNumero(model.TelefoneCelular),
                    TelefoneComercial = Util.SomenteNumero(model.TelefoneComercial)
                };

                if (_userManager.Users.Where(x => x.CNPJ == user.CNPJ || x.Email.ToUpper() == user.Email.ToUpper()).Any())
                {
                    ModelState.AddModelError(string.Empty, "CNPJ ou E-mail já cadastrados");
                }
                else
                {
                    var result = await CadastrarCliente(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(model);
        }

        private async Task<IdentityResult> CadastrarCliente(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Cliente");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.CreatePasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailRegisterAsync(user.Email, user.RazaoSocial, user.UserName, code, callbackUrl);
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var sr = new StreamReader(file.OpenReadStream());

                var dt = Util.ConvertCSVtoDataTable(sr);

                if (dt == null)
                {
                    TempData["Error"] = "Arquivo em branco";
                }
                else
                {
                    if (Util.validaColunas(Dicionario.RetornaColunasClientes(), dt.Columns))
                    {
                        var clientes = PopulaListaClientes(dt);

                        var mensagem = ValidaInformacoes(clientes);

                        if (Util.validaCampo(mensagem))
                        {
                            TempData["Error"] = mensagem;
                        }
                        else
                        {
                            foreach (var cliente in clientes)
                            {
                                try
                                {
                                    var result = await CadastrarCliente(cliente);

                                    if (!result.Succeeded)
                                    {
                                        TempData["Error"] = result.Errors;

                                        break;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    var user = await _userManager.FindByEmailAsync(cliente.Email);

                                    if (user != null)
                                    {
                                        await _userManager.DeleteAsync(user);
                                    }

                                    throw ex;
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "As colunas do arquivo estão inválidas";
                    }
                }
            }
            catch
            {
                TempData["Error"] = "Algum erro desconhecido ocorreu, tente novamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private string ValidaInformacoes(IList<ApplicationUser> clientes)
        {
            string mensagem = "";

            foreach (var cliente in clientes)
            {
                if (_userManager.Users.Where(x => x.CNPJ == cliente.CNPJ || x.Email.ToUpper() == cliente.Email.ToUpper()).Any()) mensagem = "CNPJ ou E-mail já cadastrados";
                else if (!Util.validaCampo(cliente.Nome)) mensagem = "Campo Nome é obrigatório";
                else if (!Util.validaCampo(cliente.RazaoSocial)) mensagem = "Campo Razão Social é obrigatório";
                else if (!Util.validaCampo(cliente.Email)) mensagem = "Campo Email é obrigatório e deve estar no padrão correto";
                else if (!Util.validaCampo(cliente.CNPJ) || cliente.TelefoneComercial.Length > 14) mensagem = "Campo CNPJ é obrigatório e deve conter 14 caracteres";
                else if (!Util.validaCampo(cliente.TelefoneCelular) || cliente.TelefoneCelular.Length != 11) mensagem = "Campo Telefone Celular é obrigatório e deve conter 11 caracteres";
                else if (!Util.validaCampo(cliente.TelefoneComercial) || cliente.TelefoneComercial.Length < 10 || cliente.TelefoneComercial.Length > 11) mensagem = "Campo Telefone Comercial é obrigatório e deve conter 10 ou 11 caracteres";
                else if (!Util.validaCampo(cliente.Cidade)) mensagem = "Campo Cidade é obrigatório";
                else if (!Util.validaCampo(cliente.Estado) || cliente.Estado.Length != 2) mensagem = "Campo Estado é obrigatório e deve conter 2 caracteres";
                else if (!Util.validaCampo(cliente.CEP) || cliente.CEP.Length != 8) mensagem = "Campo CEP é obrigatório e deve conter 8 caracteres";

                if (Util.validaCampo(mensagem)) return cliente.CNPJ + ": " + mensagem;
            }

            return null;
        }

        private List<ApplicationUser> PopulaListaClientes(DataTable dt)
        {
            List<ApplicationUser> lista = new List<ApplicationUser>();

            foreach (DataRow linha in dt.Rows)
            {
                ApplicationUser notaFiscal = new ApplicationUser();

                notaFiscal.UserName = Util.validaCampo(linha["CNPJ"]) ? Util.SomenteNumero(linha["CNPJ"].ToString()) : string.Empty;
                notaFiscal.Nome = Util.validaCampo(linha["Nome"]) ? linha["Nome"].ToString() : string.Empty;
                notaFiscal.RazaoSocial = Util.validaCampo(linha["RazaoSocial"]) ? linha["RazaoSocial"].ToString() : string.Empty;
                notaFiscal.Email = Util.validaCampo(linha["Email"]) ? linha["Email"].ToString() : string.Empty;
                notaFiscal.CNPJ = Util.validaCampo(linha["CNPJ"]) ? Util.SomenteNumero(linha["CNPJ"].ToString()) : string.Empty;
                notaFiscal.TelefoneCelular = Util.validaCampo(linha["TelefoneCelular"]) ? Util.SomenteNumero(linha["TelefoneCelular"].ToString()) : string.Empty;
                notaFiscal.TelefoneComercial = Util.validaCampo(linha["TelefoneComercial"]) ? Util.SomenteNumero(linha["TelefoneComercial"].ToString()) : string.Empty;
                notaFiscal.Cidade = Util.validaCampo(linha["Cidade"]) ? linha["Cidade"].ToString() : string.Empty;
                notaFiscal.Estado = Util.validaCampo(linha["Estado"]) ? linha["Estado"].ToString() : string.Empty;
                notaFiscal.CEP = Util.validaCampo(linha["CEP"]) ? Util.SomenteNumero(linha["CEP"].ToString()) : string.Empty;

                lista.Add(notaFiscal);
            }

            return lista;
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome,RazaoSocial,Email,CNPJ,TelefoneComercial,TelefoneCelular,CEP,Cidade,Estado")] ApplicationUser cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);

                    user.Nome = cliente.Nome;
                    user.RazaoSocial = cliente.RazaoSocial;
                    user.TelefoneComercial = Util.SomenteNumero(cliente.TelefoneComercial);
                    user.TelefoneCelular = Util.SomenteNumero(cliente.TelefoneCelular);
                    user.CEP = Util.SomenteNumero(cliente.CEP);
                    user.Cidade = cliente.Cidade;
                    user.Estado = cliente.Estado;

                    await _userManager.UpdateAsync(user);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cliente = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
