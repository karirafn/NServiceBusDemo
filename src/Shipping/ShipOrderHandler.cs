using NServiceBus.Logging;

namespace Shipping;

internal class ShipOrderHandler : IHandleMessages<ShipOrder>
{
    static readonly ILog Log = LogManager.GetLogger<ShipOrderHandler>();

    public Task Handle(ShipOrder message, IMessageHandlerContext context)
    {
        Log.Info($"Order [{message.OrderId}] - Successfully shipped.");
        return Task.CompletedTask;
    }
}
