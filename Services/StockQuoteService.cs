using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using stock_quote_alert.Configurations;

namespace stock_quote_alert.Services
{
    public class StockQuoteService : BackgroundService
    {
        private readonly ILogger<StockQuoteService> _logger;
        private readonly ServiceConfigurations _serviceConfigurations;

        public StockQuoteService(ILogger<StockQuoteService> logger,
            IOptions<ServiceConfigurations> serviceConfigurations)
        {
            _serviceConfigurations = serviceConfigurations.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(TimeSpan.FromMinutes(_serviceConfigurations.Api.IntervalInMinutes), stoppingToken);
            }
        }
    }
}
