using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared;

public static class IHostBuilderExtensions
{
    public static IHostBuilder ConfigureEndpoint<T>(this IHostBuilder builder, Action<HostBuilderContext, RoutingSettings<RabbitMQTransport>>? routing = null)
        where T : class => builder
        .ConfigureLogging(LoggingConfiguration)
        .ConfigureAppConfiguration(AppConfiguration<T>)
        .ConfigureServices(RegisterOpenTelemetry)
        .UseNServiceBus(context => NServiceBusConfiguration(context, routing));

    private static void AppConfiguration<T>(IConfigurationBuilder configuration) where T : class => configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<T>();

    private static void RegisterOpenTelemetry(HostBuilderContext context, IServiceCollection services)
    {
        string endpointName = context.Configuration.GetRequiredSection("EndpointName").Value!;

        services.AddOpenTelemetry()
            .WithTracing(traceProviderBuilder => traceProviderBuilder
                .AddSource("NServiceBus.*")
                .ConfigureResource(resource => resource.AddService(endpointName))
                .AddConsoleExporter())
            .WithMetrics(meterProviderBuilder => meterProviderBuilder
                .ConfigureResource(resource => resource.AddService(endpointName))
                .AddConsoleExporter());
    }

#pragma warning disable CA1416
    private static void LoggingConfiguration(HostBuilderContext context, ILoggingBuilder logging) => logging
        .AddConfiguration(context.Configuration.GetSection("Logging"))
        .AddOpenTelemetry(loggingOptions =>
        {
            loggingOptions.IncludeFormattedMessage = true;
            loggingOptions.IncludeScopes = true;
            loggingOptions.ParseStateValues = true;
            loggingOptions.AddConsoleExporter();
        })
        .AddEventLog() // Only works on Windows
        .AddConsole();
#pragma warning restore CA1416

    private static EndpointConfiguration NServiceBusConfiguration(HostBuilderContext context, Action<HostBuilderContext, RoutingSettings<RabbitMQTransport>>? routing)
    {
        EndpointConfiguration endpointConfiguration = new(context.Configuration.GetValue<string>("EndpointName"));
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.EnableOpenTelemetry();

        TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(context.Configuration.GetConnectionString("Transport"));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(() => new SqlConnection(context.Configuration.GetConnectionString("Persistence")));

        routing?.Invoke(context, transport.Routing());

        return endpointConfiguration;
    }
}
