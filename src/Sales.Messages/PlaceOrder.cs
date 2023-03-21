namespace Sales.Messages;

public record PlaceOrder(string OrderId) : ICommand;
