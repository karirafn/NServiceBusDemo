using System.Reflection;

using Microsoft.Data.SqlClient;
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
    endpointConfiguration.EnableInstallers();

    TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.ConnectionString(context.Configuration.GetConnectionString(nameof(RabbitMQ)));
    transport.UseConventionalRoutingTopology(QueueType.Quorum);

    PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    persistence.SqlDialect<SqlDialect.MsSqlServer>();
    persistence.ConnectionBuilder(() => new SqlConnection(context.Configuration.GetConnectionString("BusDb")));

    return endpointConfiguration;
});

IHost app = builder.Build();

await app.RunAsync();