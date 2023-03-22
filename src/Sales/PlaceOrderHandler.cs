using Microsoft.Extensions.Logging;

using Sales.Messages;

namespace Sales;

internal class PlaceOrderHandler : IHandleMessages<PlaceOrder>
{
    private readonly ILogger<PlaceOrderHandler> _logger;

    public PlaceOrderHandler(ILogger<PlaceOrderHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Received {command} command for order {orderId}", nameof(PlaceOrder), message.OrderId);

        OrderPlaced orderPlaced = new(message.OrderId);

        _logger.LogInformation("Sending {event} event for order {orderId}", nameof(orderPlaced), orderPlaced.OrderId);
        return context.Publish(orderPlaced);
    }
}
