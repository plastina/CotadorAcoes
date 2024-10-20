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
            _config = config;
        }

        public void SendAlertEmail(string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_config.Smtp.Host)
                {
                    Port = _config.Smtp.Port,
                    Credentials = new NetworkCredential(_config.Smtp.Username, _config.Smtp.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config.Smtp.Username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(_config.Email.To);

                smtpClient.Send(mailMessage);

                Console.WriteLine("E-mail enviado com sucesso!");
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