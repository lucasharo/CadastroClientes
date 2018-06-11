using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CadastroClientes.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(email, "ABC Teste Ltda")
            };

            mail.To.Add(new MailAddress(email));

            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(Configuration.GetSection("ConfigEmail").GetValue<string>("smtp"),
                Configuration.GetSection("ConfigEmail").GetValue<int>("porta")))
            {
                smtp.Credentials = new NetworkCredential(Configuration.GetSection("ConfigEmail").GetValue<string>("email"),
                    Configuration.GetSection("ConfigEmail").GetValue<string>("senha"));
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }
    }
}