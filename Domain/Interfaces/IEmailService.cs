using System;

namespace CotadorAcoes.Domain.Interfaces
{
    public interface IEmailService
    {
        void SendAlertEmail(string subject, string body);
    }
}
