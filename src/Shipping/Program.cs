using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Shared;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureHost<Program>();

builder.UseNServiceBus(context =>
{
    EndpointConfiguration endpointConfiguration = new(context.Configuration.GetValue<string>("EndpointName"));
    endpointConfiguration.ConfigureEndpoint();
    
    TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.ConfigureTransport(context.Configuration);
    PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.ConfigurePersistence(context.Configuration);

    return endpointConfiguration;
});

IHost app = builder.Build();

await app.RunAsync();