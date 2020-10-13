namespace stock_quote_alert.Configurations
{
    public class SmtpConfigurations
    {
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
