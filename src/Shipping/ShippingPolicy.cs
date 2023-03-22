using Billing.Messages;

using Microsoft.Extensions.Logging;

using Sales.Messages;

using static Shipping.ShippingPolicy;

namespace Shipping;

public class ShippingPolicy : Saga<ShippingPolicyData>,
    IAmStartedByMessages<OrderPlaced>,
    IAmStartedByMessages<OrderBilled>
{
    private readonly ILogger<ShippingPolicy> _logger;

    public ShippingPolicy(ILogger<ShippingPolicy> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Received {event} event for order {orderId}", nameof(OrderPlaced), message.OrderId);
        Data.IsOrderPlaced = true;
        return ProcessOrder(context);
    }

    public Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Received {event} event for order {orderId}", nameof(OrderBilled), message.OrderId);
        Data.IsOrderBilled = true;
        return ProcessOrder(context);
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
    {
        mapper.MapSaga(saga => saga.OrderId)
            .ToMessage<OrderBilled>(msg => msg.OrderId)
            .ToMessage<OrderPlaced>(msg => msg.OrderId);
    }

    public class ShippingPolicyData : ContainSagaData
    {
        public string OrderId { get; set; } = string.Empty;
        public bool IsOrderPlaced { get; set; }
        public bool IsOrderBilled { get; set; }
    }

    private async Task ProcessOrder(IMessageHandlerContext context)
    {
        if (!Data.IsOrderPlaced || !Data.IsOrderBilled)
        {
            return;
        }

        ShipOrder shipOrder = new(Data.OrderId);

        _logger.LogInformation("Sending {command} command for order {id}", nameof(ShipOrder), shipOrder.OrderId);
        await context.SendLocal(shipOrder);

        MarkAsComplete();
    }
}
