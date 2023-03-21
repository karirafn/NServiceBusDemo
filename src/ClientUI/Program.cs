using System.Reflection;

using ClientUI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sales.Messages;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((context, configuration) => configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>());

builder.UseNServiceBus(context =>
{
    EndpointConfiguration endpointConfiguration = new(context.Configuration.GetValue<string>("EndpointName"));

    TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.ConnectionString(context.Configuration.GetConnectionString(nameof(RabbitMQ)));
    transport.UseConventionalRoutingTopology(QueueType.Quorum);

    RoutingSettings<RabbitMQTransport> routing = transport.Routing();

    routing.RouteToEndpoint(typeof(PlaceOrder), context.Configuration.GetValue<string>("Routing:SalesEndPointName"));

    return endpointConfiguration;
});

builder.ConfigureServices(services => services.AddHostedService<Worker>());

IHost app = builder.Build();

await app.RunAsync();