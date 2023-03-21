using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Shared;

public static class EndpointConfigurationExtensions
{
    public static TransportExtensions<RabbitMQTransport> ConfigureRabbitMQ(this EndpointConfiguration endpointConfiguration, IConfiguration configuration)
    {
        TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(configuration.GetConnectionString("Transport"));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        return transport;
    }

    public static PersistenceExtensions<SqlPersistence> ConfigurePersistence(this EndpointConfiguration endpointConfiguration, IConfiguration configuration)
    {
        PersistenceExtensions<SqlPersistence> persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(() => new SqlConnection(configuration.GetConnectionString("Persistence")));

        return persistence;
    }
}
