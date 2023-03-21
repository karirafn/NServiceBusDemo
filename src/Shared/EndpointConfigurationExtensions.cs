using Microsoft.Extensions.Configuration;

namespace Shared;

public static class EndpointConfigurationExtensions
{
    public static TransportExtensions<RabbitMQTransport> ConfigureRabbitMQ(this EndpointConfiguration endpointConfiguration, IConfiguration configuration)
    {
        TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(configuration.GetConnectionString(nameof(RabbitMQ)));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        return transport;
    }
}
