namespace stock_quote_alert.Abstractions
{
    public interface INotification
    {
        void send(string from, string to, string subject, string message);
    }
}