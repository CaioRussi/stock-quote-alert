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
using Microsoft.Extensions.Configuration;
using stock_quote_alert.DTO.Api.GetQuote;

namespace stock_quote_alert.Services
{
    public class StockQuoteService : BackgroundService
    {
        private readonly ILogger<StockQuoteService> _logger;
        private readonly ServiceConfigurations _serviceConfigurations;
        private readonly IStockQuoteApi _apiClient;
        private readonly INotification _notification;
        private readonly string _ativo;
        private readonly string _saleValue;
        private readonly string _purchaseValue;

        public StockQuoteService(ILogger<StockQuoteService> logger,
            IOptions<ServiceConfigurations> serviceConfigurations,
            INotification notification,
            IConfiguration args)
        {
            _serviceConfigurations = serviceConfigurations.Value;
            _logger = logger;
            _notification = notification;
            _ativo = args.GetValue<string>("ativo");
            _saleValue = args.GetValue<string>("saleValue");
            _purchaseValue = args.GetValue<string>("purchaseValue");

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

                var response = _apiClient.GetQuote(_serviceConfigurations.Api.ApiKey, _ativo).Result;

                await Task.Delay(TimeSpan.FromMinutes(_serviceConfigurations.Api.IntervalInMinutes), stoppingToken);
            }
        }

        private void checkQuoteValue()
        {
            var response = _apiClient.GetQuote(_serviceConfigurations.Api.ApiKey, _ativo).Result;
            checkErrorInApiResponse(response);

            var currentPrice = response.Results.First().Value.Price;
            if (currentPrice >= double.Parse(_saleValue))
            {
                sendNotification($"O ativo {_ativo} está favorável para a venda",
                    $"O ativo {_ativo} está com o valor de R${currentPrice}");
                _logger.LogInformation($"O ativo {_ativo} está favorável para a venda com o valor de R${currentPrice}");
            }
            else
            {
                if (currentPrice <= double.Parse(_purchaseValue))
                {
                    sendNotification($"O ativo {_ativo} está favorável para a compra",
                        $"O ativo {_ativo} está com o valor de R${currentPrice}");
                    _logger.LogInformation($"O ativo {_ativo} está favorável para a compra com o valor de R${currentPrice}");
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

        private void ValidateArguments()
        {
            if (string.IsNullOrWhiteSpace(_ativo))
            {
                throw new ArgumentNullException("The parameter \"ativo\" value must not be null or empty. Use --ativo={value} to set a value");
            }

            if (string.IsNullOrWhiteSpace(_saleValue))
            {
                throw new ArgumentNullException("The parameter \"saleValue\" value must not be null or empty. Use --saleValue={value} to set a value");
            }

            if (string.IsNullOrWhiteSpace(_purchaseValue))
            {
                throw new ArgumentNullException("The parameter \"purchaseValue\" value must not be null or empty. Use --purchaseValue={value} to set a value");
            }

            if (double.Parse(_saleValue) <= double.Parse(_purchaseValue))
            {
                throw new ArgumentException("The \"saleValue\" must be greater than \"purchaseValue\"");
            }
        }

        private void checkErrorInApiResponse(ResponseGetQuoteDto response)
        {
            var result = response.Results.First().Value;
            if (result.Error == true)
            {
                throw new Exception(result.Message);
            }
        }
    }
}
