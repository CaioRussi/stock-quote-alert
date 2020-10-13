using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using stock_quote_alert.Abstractions;
using stock_quote_alert.Configurations;
using Refit;

namespace stock_quote_alert.Services
{
    public class StockQuoteService : BackgroundService
    {
        private readonly ILogger<StockQuoteService> _logger;
        private readonly ServiceConfigurations _serviceConfigurations;
        private readonly IStockQuoteApi _apiClient;

        public StockQuoteService(ILogger<StockQuoteService> logger,
            IOptions<ServiceConfigurations> serviceConfigurations)
        {
            _serviceConfigurations = serviceConfigurations.Value;
            _logger = logger;

            _apiClient = RestService.For<IStockQuoteApi>(_serviceConfigurations.Api.BaseUrl);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var response = _apiClient.GetQuote(_serviceConfigurations.Api.ApiKey, "petr4").Result;

                await Task.Delay(TimeSpan.FromMinutes(_serviceConfigurations.Api.IntervalInMinutes), stoppingToken);
            }
        }
    }
}
