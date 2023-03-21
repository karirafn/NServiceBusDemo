using Billing.Messages;

using NServiceBus.Logging;

using Sales.Messages;

namespace Billing;

internal class OrderPlacedHandler : IHandleMessages<OrderPlaced>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderPlacedHandler>();

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        Log.Info($"Received {nameof(OrderPlaced)} with {nameof(OrderPlaced.OrderId)}: {message.OrderId}");

        OrderBilled orderBilled = new(message.OrderId);

        return context.Publish(orderBilled);
    }
}
