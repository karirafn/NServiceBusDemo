namespace Sales.Messages;

public record OrderPlaced(string OrderId) : IEvent;
