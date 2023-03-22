using Microsoft.Extensions.Logging;

namespace Shipping;

internal class ShipOrderHandler : IHandleMessages<ShipOrder>
{
    private readonly ILogger<ShipOrderHandler> _logger;

    public ShipOrderHandler(ILogger<ShipOrderHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ShipOrder message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Order {orderId} - Successfully shipped.", message.OrderId);
        return Task.CompletedTask;
    }
}
