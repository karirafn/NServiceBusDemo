using System.Reflection;

using Microsoft.Extensions.Hosting;

using Shared;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureHost<Program>();

builder.UseNServiceBus(context =>
{
    EndpointConfiguration endpointConfiguration = EndpointConfigurationFactory.Create(context.Configuration);
    TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.ConfigureTransport(context.Configuration);
    PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.ConfigurePersistence(context.Configuration);

    return endpointConfiguration;
});

IHost app = builder.Build();

await app.RunAsync();