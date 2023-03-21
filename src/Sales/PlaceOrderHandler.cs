using NServiceBus.Logging;

using Sales.Messages;

namespace Sales;

internal class PlaceOrderHandler : IHandleMessages<PlaceOrder>
{
    private static readonly ILog Log = LogManager.GetLogger<PlaceOrderHandler>();

    public Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        Log.Info($"Received {nameof(PlaceOrder)} with {nameof(PlaceOrder.OrderId)}: {message.OrderId}");

        OrderPlaced orderPlaced = new(message.OrderId);

        return context.Publish(orderPlaced);
    }
}
