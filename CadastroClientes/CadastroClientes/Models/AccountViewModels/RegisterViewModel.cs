using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroClientes.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Nome Empersa")]
        public string RazaoSocial { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefone Comercial")]
        public string TelefoneComercial { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefone Celular")]
        public string TelefoneCelular { get; set; }

        [Required]
        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Required]
        [Display(Name = "Estado (UF)")]
        [StringLength(2)]
        [RegularExpression("[a-zA-Z]{2}")]
        public string Estado { get; set; }
        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password(ex: @Abc123)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
