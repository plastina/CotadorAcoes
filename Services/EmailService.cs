using CotadorAcoes.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace CotadorAcoes.Services
{
    public class EmailService
    {
        private readonly AppSettings _config;

        public EmailService(AppSettings config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "Configurações de e-mail não podem ser nulas.");
        }

        public void SendAlertEmail(string subject, string body)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_config.Smtp.Host))
                {
                    smtpClient.Port = _config.Smtp.Port;
                    smtpClient.Credentials = new NetworkCredential(_config.Smtp.Usuario, _config.Smtp.Senha);
                    smtpClient.EnableSsl = _config.Smtp.UseSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_config.Smtp.Usuario);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;

                        mailMessage.To.Add(_config.Email.Destinatario);

                        smtpClient.Send(mailMessage);
                    }
                }

                Console.WriteLine("E-mail enviado com sucesso!");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Erro de SMTP ao enviar e-mail: {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine($"Detalhes: {smtpEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalhes: {ex.InnerException.Message}");
                }
            }
        }
    }
}