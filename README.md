# NServiceBusDemo

This project is based on the [NServiceBus Step-by-step tutoriial](https://docs.particular.net/tutorials/nservicebus-step-by-step/). I've modified it to use the `IHostBuilder` to register NServiceBus instead of the default implementation. I have also implemented [OpenTelemetry](https://opentelemetry.io/) and the docker stack contains both the [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) as well as some backends.

## Getting started

1. Run the following PowerShell command:

    ```pwsh
    ('traces', 'metrics', 'logs') | Foreach-Object {New-Item "C:\temp\otel\$_.json"}
    ```

1. Start docker containers.

    ```bash
    docker-compose up
    ```

1. Add this JSON to user secrets for the `ClientUI` project.

    ```json
    {
      "ConnectionStrings": {
        "Persistence": "<SqlConnectionString>",
        "Transport": "<RabbitMqConnectionString>"
      }
    }
    ```

1. Connect to the SQL database with the username and password defined in `docker-compose.yml`.
1. Add the `NServiceBusDemo` database.
1. Run `Billing`, `ClientUI`, `Sales` and `Shipping` and send messages from the `ClientUI` window.
1. Go to <http://localhost:16686> to view traces in [Jaeger](https://www.jaegertracing.io/).
1. Go to <http://localhost:9090> to view metrics in [Prometheus](https://prometheus.io/).
