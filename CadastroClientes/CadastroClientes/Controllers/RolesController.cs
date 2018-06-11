
using System.Linq;
using CadastroClientes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CAdastroClientes.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var Roles = _context.Roles.ToList();
            return View(Roles);
        }
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            _context.Roles.Add(Role);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}