using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CadastroClientes.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }
        
        [Display(Name = "Nome Empersa")]
        public string RazaoSocial { get; set; }
        
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }
        
        [Phone]
        [Display(Name = "Telefone Comercial")]
        public string TelefoneComercial { get; set; }
        
        [Phone]
        [Display(Name = "Telefone Celular")]
        public string TelefoneCelular { get; set; }
        
        [Display(Name = "CEP")]
        public string CEP { get; set; }
        
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
        
        [Display(Name = "Estado (UF)")]
        [StringLength(2)]
        [RegularExpression("[a-zA-Z]{2}")]
        public string Estado { get; set; }

        [NotMapped]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password (ex: @Abc123)")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public string StatusMessage { get; set; }
    }
}