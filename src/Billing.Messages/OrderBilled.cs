namespace Billing.Messages;

public record OrderBilled(string OrderId) : IEvent;
