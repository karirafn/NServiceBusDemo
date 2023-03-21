using Microsoft.Extensions.Hosting;

using NServiceBus.Logging;

using Sales.Messages;

namespace ClientUI;

internal class Worker : BackgroundService
{
    private readonly IMessageSession _messageSession;
    private static readonly ILog Log = LogManager.GetLogger<Worker>();

    public Worker(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            Log.Info("Press 'P' to place an order, or 'Q' to quit");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.P:
                    PlaceOrder command = new(Guid.NewGuid().ToString());

                    Log.Info($"Sending {nameof(PlaceOrder)} command with {nameof(PlaceOrder.OrderId)}: {command.OrderId}");
                    await _messageSession.Send(command, cancellationToken: stoppingToken)
                        .ConfigureAwait(false);

                    break;

                case ConsoleKey.Q:
                    return;

                default:
                    Log.Info("Unknown input. Please try again.");
                    break;
            }
        }
    }
}
