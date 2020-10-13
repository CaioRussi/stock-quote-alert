using System.Threading.Tasks;
using stock_quote_alert.DTO.Api.GetQuote;
using Refit;

namespace stock_quote_alert.Abstractions
{
    public interface IStockQuoteApi
    {
        [Get("/stock_price?key={key}&symbol={symbol}")]
        Task<ResponseGetQuoteDto> GetQuote(string key, string symbol);
    }
}
