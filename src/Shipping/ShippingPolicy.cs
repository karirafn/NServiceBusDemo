using Billing.Messages;

using NServiceBus.Logging;

using Sales.Messages;

using static Shipping.ShippingPolicy;

namespace Shipping;

public class ShippingPolicy : Saga<ShippingPolicyData>,
    IAmStartedByMessages<OrderPlaced>,
    IAmStartedByMessages<OrderBilled>
{
    private static readonly ILog Log = LogManager.GetLogger<ShippingPolicy>();

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        Log.Info($"Received {nameof(OrderPlaced)} with {nameof(OrderPlaced.OrderId)}: {message.OrderId}");
        Data.IsOrderPlaced = true;
        return ProcessOrder(context);
    }

    public Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        Log.Info($"Received {nameof(OrderBilled)} with {nameof(OrderBilled.OrderId)}: {message.OrderId}");
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

        await context.SendLocal(new ShipOrder(Data.OrderId));
        MarkAsComplete();
    }
}
