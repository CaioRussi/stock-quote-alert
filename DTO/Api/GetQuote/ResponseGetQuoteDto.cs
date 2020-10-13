using System.Collections.Generic;

namespace stock_quote_alert.DTO.Api.GetQuote
{
    public class ResponseGetQuoteDto
    {
        public string By { get; set; }
        public bool ValidKey { get; set; }
        public Dictionary<string, ResponseBodyDto> Results { get; set; }
        public double ExecutionTime { get; set; }
        public bool FromCache { get; set; }
    }
}