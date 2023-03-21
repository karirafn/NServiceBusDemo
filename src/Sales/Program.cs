using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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

    return endpointConfiguration;
});

IHost app = builder.Build();

await app.RunAsync();