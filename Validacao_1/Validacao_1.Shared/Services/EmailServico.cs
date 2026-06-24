using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Validacao_1.Shared.Services
{
    public class EmailServico
    {
        private readonly IConfiguration _config;
        public EmailServico(IConfiguration config)
        {
            _config = config;
        }

        [UnsupportedOSPlatform("browser")]
        public async Task EnviarEmailCodigo(string emailDoCliente, int codigo, string remetente, string usuarioSmtp, string suaSenhaDeApp)
        {
            var mensagem = new MailMessage();
            mensagem.From = new MailAddress(remetente, "Sistema de Cadastro");
            mensagem.To.Add(new MailAddress(emailDoCliente));
            mensagem.Subject = "Código de Verificação";
            mensagem.Body = $"Seu código de verificação é: {codigo}";
            mensagem.IsBodyHtml = false;

            using var smtpClient = new SmtpClient("smtp-relay.brevo.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(usuarioSmtp, suaSenhaDeApp),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mensagem);
        }
    }
}
