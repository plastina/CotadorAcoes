using CotadorAcoes.Configuration;
using CotadorAcoes.Domain.Interfaces;
using System;
using System.Net;
using System.Net.Mail;

namespace CotadorAcoes.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _config;

        public EmailService(AppSettings config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "Configurações de e-mail não podem ser nulas.");
        }

        public void EnviarEmailAlerta(string subject, string body)
        {
            if (string.IsNullOrEmpty(_config.Smtp.Host) ||
                string.IsNullOrEmpty(_config.Smtp.Usuario) ||
                string.IsNullOrEmpty(_config.Smtp.Senha) ||
                string.IsNullOrEmpty(_config.Email.Destinatario))
            {
                throw new ArgumentNullException("Configuração SMTP ou destinatário está ausente.");
            }

            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_config.Smtp.Host))
                {
                    smtpClient.Port = _config.Smtp.Port;
                    smtpClient.Credentials = new NetworkCredential(_config.Smtp.Usuario, _config.Smtp.Senha);
                    smtpClient.EnableSsl = _config.Smtp.UseSsl;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_config.Smtp.Usuario);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.To.Add(_config.Email.Destinatario);
                        mailMessage.IsBodyHtml = true;

                        smtpClient.Send(mailMessage);

                        Console.WriteLine("Email enviado com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}
