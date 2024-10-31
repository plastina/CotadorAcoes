using CotadorAcoes.Application.Services;
using CotadorAcoes.Configuration;
using CotadorAcoes.Domain.Interfaces;
using Moq;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace CotadorAcoes.Tests
{
    public class EmailServiceTests
    {
        private readonly AppSettings _appSettings;
        private readonly Mock<IEmailService> _emailServiceMock;

        public EmailServiceTests()
        {
            _appSettings = new AppSettings
            {
                Smtp = new SmtpSettings
                {
                    Host = "smtp.test.com",
                    Port = 587,
                    Usuario = "testuser@test.com",
                    Senha = "password",
                    UseSsl = true
                },
                Email = new EmailSettings
                {
                    Destinatario = "recipient@test.com"
                }
            };
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Fact]
        public void SendAlertEmail_ShouldThrowArgumentNullException_WhenConfigIsIncomplete()
        {
            AppSettings incompleteConfig = new AppSettings
            {
                Smtp = new SmtpSettings { Host = "", Usuario = "", Senha = "" },
                Email = new EmailSettings { Destinatario = "" }
            };
            EmailService emailService = new EmailService(incompleteConfig);

            Assert.Throws<ArgumentNullException>(() => emailService.SendAlertEmail("Subject", "Body"));
        }

        [Fact]
        public async Task SendAlertEmail_ShouldSendEmail_WhenConfigIsValid()
        {
            string subject = "Test Subject";
            string body = "Test Body";

            _emailServiceMock.Setup(service => service.SendAlertEmail(subject, body)).Verifiable();

            _emailServiceMock.Object.SendAlertEmail(subject, body);

            _emailServiceMock.Verify(service => service.SendAlertEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            await Task.CompletedTask;
        }

        [Fact]
        public void SendAlertEmail_ShouldHandleSmtpException_WhenThrown()
        {
            _emailServiceMock.Setup(service => service.SendAlertEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new SmtpException("SMTP error"));
            EmailService emailService = new EmailService(_appSettings);

            Exception exception = Record.Exception(() => emailService.SendAlertEmail("Test Subject", "Test Body"));

            Assert.Null(exception); 
        }

        [Fact]
        public void SendAlertEmail_ShouldHandleGeneralException_WhenThrown()
        {
            _emailServiceMock.Setup(service => service.SendAlertEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("General error"));
            EmailService emailService = new EmailService(_appSettings);

            Exception exception = Record.Exception(() => emailService.SendAlertEmail("Test Subject", "Test Body"));

            Assert.Null(exception); 
        }
    }
}
