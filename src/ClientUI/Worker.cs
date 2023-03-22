using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Sales.Messages;

namespace ClientUI;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageSession _messageSession;

    public Worker(ILogger<Worker> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            _logger.LogInformation("Press {P} to place an order, or {Q} to quit", ConsoleKey.P, ConsoleKey.Q);
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.P:
                    PlaceOrder command = new(Guid.NewGuid().ToString());

                    _logger.LogInformation("Sending {command} command for order {id}", nameof(PlaceOrder), command.OrderId);
                    await _messageSession.Send(command, cancellationToken: stoppingToken)
                        .ConfigureAwait(false);

                    break;

                case ConsoleKey.Q:
                    return;

                default:
                    _logger.LogInformation("Unknown input. Please try again.");
                    break;
            }
        }
    }
}
