using System.Reflection;

using Microsoft.Extensions.Hosting;

using Shared;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

await Host.CreateDefaultBuilder(args)
    .ConfigureEndpoint<Program>()
    .Build()
    .RunAsync();
