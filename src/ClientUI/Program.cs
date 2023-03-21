using System.Reflection;

using ClientUI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sales.Messages;

using Shared;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

await Host.CreateDefaultBuilder(args)
    .ConfigureEndpoint<Program>((context, routing) => routing
        .RouteToEndpoint(typeof(PlaceOrder), context.Configuration.GetValue<string>("Routing:SalesEndPointName")))
    .ConfigureServices(services => services.AddHostedService<Worker>())
    .Build()
    .RunAsync();