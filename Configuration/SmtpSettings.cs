namespace CotadorAcoes.Configuration
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool UseSsl { get; set; }
    }
}