namespace CotadorAcoes.Configuration
{
    public class EmailSettings
    {
        public string To { get; set; }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }

    public class AppSettings
    {
        public EmailSettings Email { get; set; }
        public SmtpSettings Smtp { get; set; }
        public ApiSettings Api { get; set; }
    }
}