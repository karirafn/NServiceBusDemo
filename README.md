# NServiceBusDemo

## Getting started

1. Have these services up and running.
    - [RabbitMQ](https://www.rabbitmq.com/)
    - [OpenTelemetry collector](https://opentelemetry.io/docs/collector/)
    - [Jaeger](https://www.jaegertracing.io/)
    - [SQL server](https://www.microsoft.com/en-us/sql-server)
1. Add this JSON to user secrets for the `Billing`, `ClientUI`, `Sales` and `Shipping` projects.

    ```json
    {
      "ConnectionStrings": {
        "Persistence": "<SqlConnectionString>",
        "Transport": "<RabbitMqConnectionString>"
      }
    }
    ```

1. Install the NServiceBus RabbitMQ transport CLI tool.

   ```bash
   dotnet tool install -g NServiceBus.Transport.RabbitMQ.CommandLine
   ```

1. Add queues to RabbitMQ.

    ```bash
    rabbitmq-transport endpoint create "NServiceBusDemo.ClientUI" -c "<RabbitMqConnectionString>"
    rabbitmq-transport endpoint create "NServiceBusDemo.Billing" -c "<RabbitMqConnectionString>"
    rabbitmq-transport endpoint create "NServiceBusDemo.Sales" -c "<RabbitMqConnectionString>"
    rabbitmq-transport endpoint create "NServiceBusDemo.Shipping" -c "<RabbitMqConnectionString>"
    ```

1. Update `config.yaml` for the OpenTelemetry collector.

    ```yaml
    receivers:
    otlp:
      protocols:
        grpc:
        http:

    processors:
      batch:

    extensions:
      health_check:
      pprof:
      zpages:

    exporters:
      logging:
      jaeger:
        endpoint: localhost:14250

    service:
      extensions: [health_checks, pprof, zpages]
      pipelines:
        traces:
          receivers: [otlp]
          processors: [batch]
          exporters: [logging, jaeger]
        metrics:
          receivers: [otlp]
          processors: [batch]
          exporters: [logging]
        logs:
          receivers: [otlp]
          processors: [batch]
          exporters: [logging, jaeger]
    ```

1. Create an `NServiceBusDemo` database on your SQL server.
1. Start the OpenTelemetry collector.
1. Start Jaeger with the `--collector.otlp.enabled` parameter.
1. Run `Billing`, `ClientUI`, `Sales` and `Shipping` and send messages from the `ClientUI` window.
1. Go to <http://localhost:16686> to view traces in Jaeger.
