using Microsoft.Extensions.Configuration;

namespace Shared;

public class EndpointConfigurationFactory
{
    public static EndpointConfiguration Create(IConfiguration configuration)
    {
        EndpointConfiguration endpointConfiguration = new(configuration.GetValue<string>("EndpointName"));
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.EnableOpenTelemetry();

        return endpointConfiguration;
    }
}
