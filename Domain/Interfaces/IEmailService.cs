using System;

namespace CotadorAcoes.Domain.Interfaces
{
    public interface IEmailService
    {
        void EnviarEmailAlerta(string subject, string body);
    }
}
