using Billing.Messages;

using Microsoft.Extensions.Logging;

using Sales.Messages;

namespace Billing;

internal class OrderPlacedHandler : IHandleMessages<OrderPlaced>
{
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Received {command} command for order {orderId}", nameof(OrderPlaced), message.OrderId);

        OrderBilled orderBilled = new(message.OrderId);

        _logger.LogInformation("Sending {event} event for order {orderId}", nameof(OrderBilled), orderBilled.OrderId);
        return context.Publish(orderBilled);
    }
}
