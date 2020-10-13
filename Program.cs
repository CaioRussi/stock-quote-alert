using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using stock_quote_alert.Configurations;

namespace stock_quote_alert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ServiceConfigurations>(hostContext.Configuration.GetSection("ServiceConfigurations"));
                    services.AddTransient<ServiceConfigurations>(_ => _.GetRequiredService<IOptions<ServiceConfigurations>>().Value);
                    services.AddHostedService<Worker>();
                });
    }
}
