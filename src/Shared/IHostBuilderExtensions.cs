using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Shared;

public static class IHostBuilderExtensions
{
    public static IHostBuilder ConfigureHost<T>(this IHostBuilder builder)
        where T : class
    {
        builder.ConfigureLogging((context, logging) => logging.ConfigureOpenTelemetryLogging(context.Configuration));

        builder.ConfigureAppConfiguration((context, configuration) => configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<T>());

        builder.ConfigureServices((context, services) => services.AddNServiceBusOpenTelemetry(context.Configuration));

        return builder;
    }
}
