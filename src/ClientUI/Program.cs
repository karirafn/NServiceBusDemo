using System.Reflection;

using ClientUI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sales.Messages;

using Shared;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging((context, logging) => logging.ConfigureOpenTelemetryLogging(context.Configuration));

builder.ConfigureAppConfiguration((context, configuration) => configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>());

builder.ConfigureServices((context, services) => services.AddNServiceBusOpenTelemetry(context.Configuration));

builder.UseNServiceBus(context =>
{
    EndpointConfiguration endpointConfiguration = new(context.Configuration.GetValue<string>("EndpointName"));
    endpointConfiguration.ConfigureEndpoint();

    TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.ConfigureTransport(context.Configuration);

    RoutingSettings<RabbitMQTransport> routing = transport.Routing();

    routing.RouteToEndpoint(typeof(PlaceOrder), context.Configuration.GetValue<string>("Routing:SalesEndPointName"));

    return endpointConfiguration;
});

builder.ConfigureServices(services => services.AddHostedService<Worker>());

IHost app = builder.Build();

await app.RunAsync();