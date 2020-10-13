namespace stock_quote_alert.DTO.Api.GetQuote
{
    public class ResponseMarketTimeDto
    {
        public string Open { get; set; }
        public string Close { get; set; }
        public int Timezone { get; set; }
    }
}