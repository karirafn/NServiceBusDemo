# NServiceBusDemo

## Getting started

1. Clone repository.
1. Get the following services up and running.
    - [RabbitMQ](https://www.rabbitmq.com/).
    - [Jaeger](https://www.jaegertracing.io/).
    - [SQL server](https://www.microsoft.com/en-us/sql-server).
1. Add the following to user secrets for `Billing`, `ClientUI`, `Sales` and `Shipping`

  ```json
  {
    "ConnectionStrings": {
      "Persistence": "<SqlConnectionString>",
      "Transport": "<RabbitMqConnectionString>"
    }
  }
  ```

1. Install the NServiceBus RabbitMQ transport CLI tool

 ```bash
 dotnet tool install -g NServiceBus.Transport.RabbitMQ.CommandLine
 ```

1. Add the following queues to RabbitMQ

  ```bash
  rabbitmq-transport endpoint create "NServiceBusDemo.ClientUI" -c "<RabbitMqConnectionString>"
  rabbitmq-transport endpoint create "NServiceBusDemo.Billing" -c "<RabbitMqConnectionString>"
  rabbitmq-transport endpoint create "NServiceBusDemo.Sales" -c "<RabbitMqConnectionString>"
  rabbitmq-transport endpoint create "NServiceBusDemo.Shipping" -c "<RabbitMqConnectionString>"
  ```

1. Create an `NServiceBusDemo` database on your SQL server.
1. In Visual Studio, configure the solution to have multiple startup projects and set `Billing`, `ClientUI`, `Sales` and `Shipping` to strart.
1. Run the program and send messages.
1. Go to [Jaeger](http://localhost:16686) to view traces.
