using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CadastroClientes.Services;

namespace CadastroClientes.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static Task SendEmailRegisterAsync(this IEmailSender emailSender, string email, string nome, string userName, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Cadastrar Senha",
               $"Seja bem vindo, {nome}!!!" +
               $"<br>" +
               $"<br>" +
               $"Seu UserName é {userName}" +
               $"<br>" +
               $"<br>" +
               $"Por favor, click no link para cadastrar uma senha: <a href='{callbackUrl}'>link</a>");
        }

        public static Task SendEmailResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Resetar Senha",
               $"Por favor, click no link para resetar sua senha: <a href='{callbackUrl}'>link</a>");
        }
    }
}


