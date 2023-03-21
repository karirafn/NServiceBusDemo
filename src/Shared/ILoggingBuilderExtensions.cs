using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;

namespace Shared;

public static class ILoggingBuilderExtensions
{
    public static ILoggingBuilder ConfigureOpenTelemetryLogging(this ILoggingBuilder logging, IConfiguration configuration)
    {
        logging.AddConfiguration(configuration.GetSection("Logging"));

        logging.AddOpenTelemetry(loggingOptions =>
        {
            loggingOptions.IncludeFormattedMessage = true;
            loggingOptions.IncludeScopes = true;
            loggingOptions.ParseStateValues = true;
            loggingOptions.AddConsoleExporter();
        });

#pragma warning disable CA1416
        logging.AddEventLog();
#pragma warning restore CA1416
        logging.AddConsole();

        return logging;
    }
}
