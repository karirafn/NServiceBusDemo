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
        _logger.LogInformation("Received {command} command with order id: {orderId}", nameof(PlaceOrder), message.OrderId);

        OrderPlaced orderPlaced = new(message.OrderId);

        return context.Publish(orderPlaced);
    }
}
