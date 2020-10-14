using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using stock_quote_alert.Abstractions;
using stock_quote_alert.Configurations;
using Refit;
using System.Linq;

namespace stock_quote_alert.Services
{
    public class StockQuoteService : BackgroundService
    {
        private readonly ILogger<StockQuoteService> _logger;
        private readonly ServiceConfigurations _serviceConfigurations;
        private readonly IStockQuoteApi _apiClient;
        private readonly INotification _notification;

        public StockQuoteService(ILogger<StockQuoteService> logger,
            IOptions<ServiceConfigurations> serviceConfigurations,
            INotification notification)
        {
            _serviceConfigurations = serviceConfigurations.Value;
            _logger = logger;
            _notification = notification;

            _apiClient = RestService.For<IStockQuoteApi>(_serviceConfigurations.Api.BaseUrl);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    checkQuoteValue();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }

                var response = _apiClient.GetQuote(_serviceConfigurations.Api.ApiKey, "petr4").Result;

                await Task.Delay(TimeSpan.FromMinutes(_serviceConfigurations.Api.IntervalInMinutes), stoppingToken);
            }
        }

        private void checkQuoteValue()
        {
            var response = _apiClient.GetQuote(_serviceConfigurations.Api.ApiKey, "petr4").Result;

            var currentPrice = response.Results.First().Value.Price;
            if (currentPrice >= 15)
            {
                sendNotification($"O ativo petr4 está favorável para a venda",
                    $"O ativo petr4 está com o valor de R${currentPrice}");
                _logger.LogInformation($"O ativo petr4 está favorável para a venda com o valor de R${currentPrice}");
            }
            else
            {
                if (currentPrice <= 10)
                {
                    sendNotification($"O ativo petr4 está favorável para a compra",
                        $"O ativo petr4 está com o valor de R${currentPrice}");
                    _logger.LogInformation($"O ativo petr4 está favorável para a compra com o valor de R${currentPrice}");
                }
            }
        }

        private void sendNotification(string subject, string message)
        {
            _notification.send(
                _serviceConfigurations.Smtp.EmailFrom,
                _serviceConfigurations.Smtp.EmailTo,
                subject,
                message
            );
        }
    }
}
