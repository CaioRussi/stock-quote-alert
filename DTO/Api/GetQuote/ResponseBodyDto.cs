namespace stock_quote_alert.DTO.Api.GetQuote
{
    public class ResponseBodyDto
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Currency { get; set; }
        public ResponseMarketTimeDto MarketTime { get; set; }
        public double MarketCap { get; set; }
        public double Price { get; set; }
        public double ChangePercent { get; set; }
        public string UpdatedAt { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}