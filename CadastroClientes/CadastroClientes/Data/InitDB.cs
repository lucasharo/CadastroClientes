using CadastroClientes.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroClientes.Data
{
    public class InitDB
    {
        public static IEnumerable<ApplicationUser> GetAdmins()
        {
            return new List<ApplicationUser> {
                new ApplicationUser { UserName = "Admin", Nome = "Admin", Email = "admin@abcteste.com.br", Password = "Admin@Teste123" }
            };
        }

        public static IEnumerable<IdentityRole> GetRoles()
        {
            return new List<IdentityRole> {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "Vendedor", NormalizedName = "VENDEDOR" },
                new IdentityRole { Name = "Cliente", NormalizedName = "CLIENTE" }
            };
        }
    }
}
