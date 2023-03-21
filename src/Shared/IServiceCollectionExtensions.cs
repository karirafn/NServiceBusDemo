using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNServiceBusOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration.GetValue<string>("EndpointName")!))
                    .AddSource("NServiceBus.*")
                    .AddConsoleExporter());

        return services;
    }
}
