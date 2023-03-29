# NServiceBusDemo

## Getting started

1. Create the following files:

    ```text
    C:\temp\traces.json
    C:\temp\metrics.json
    C:\temp\logs.json
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
1. Go to <http://localhost:16686> to view traces in Jaeger.
