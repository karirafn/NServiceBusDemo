using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;

namespace Shared;

public static class IHostBuilderExtensions
{
    public const string EndpointName = nameof(EndpointName);
    public const string Persistence = nameof(Persistence);
    public const string Transport = nameof(Transport);
    public const string Logging = nameof(Logging);

    public static IHostBuilder ConfigureEndpoint<T>(this IHostBuilder builder, Action<HostBuilderContext, RoutingSettings<RabbitMQTransport>>? routing = null)
        where T : class => builder
        .ConfigureLogging(LoggingConfiguration)
        .ConfigureAppConfiguration(AppConfiguration<T>)
        .ConfigureServices(RegisterOpenTelemetry)
        .UseNServiceBus(context => NServiceBusConfiguration(context, routing))
        .UseSerilog((context, services, logConfiguration) => logConfiguration.ReadFrom.Configuration(context.Configuration));

    private static void AppConfiguration<T>(IConfigurationBuilder configuration) where T : class => configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<T>();

    private static void RegisterOpenTelemetry(HostBuilderContext context, IServiceCollection services)
    {
        ResourceBuilder resourceBuilder = CreateResourceBuilder(context);

        services.AddOpenTelemetry()
            .WithTracing(traceProviderBuilder => traceProviderBuilder
                .AddSource("NServiceBus.*")
                .SetResourceBuilder(resourceBuilder)
                .AddProcessor(new NetHostProcessor())
                .AddOtlpExporter()
                .AddConsoleExporter())
            .WithMetrics(meterProviderBuilder => meterProviderBuilder
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter()
                .AddConsoleExporter());
    }

#pragma warning disable CA1416
    private static void LoggingConfiguration(HostBuilderContext context, ILoggingBuilder logging)
    {
        ResourceBuilder resourceBuilder = CreateResourceBuilder(context);

        logging.AddConfiguration(context.Configuration.GetSection(Logging))
            .AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddConsoleExporter();
            })
            .AddEventLog() // Only works on Windows
            .AddConsole();
    }
#pragma warning restore CA1416

    private static EndpointConfiguration NServiceBusConfiguration(HostBuilderContext context, Action<HostBuilderContext, RoutingSettings<RabbitMQTransport>>? routing)
    {
        EndpointConfiguration endpointConfiguration = new(context.Configuration.GetValue<string>(EndpointName));
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.EnableOpenTelemetry();

        TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(context.Configuration.GetConnectionString(Transport));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(() => new SqlConnection(context.Configuration.GetConnectionString(Persistence)));

        routing?.Invoke(context, transport.Routing());

        return endpointConfiguration;
    }

    private static ResourceBuilder CreateResourceBuilder(HostBuilderContext context)
    {
        string endpointName = context.Configuration.GetRequiredSection(EndpointName).Value!;
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(endpointName);

        return resourceBuilder;
    }
}
