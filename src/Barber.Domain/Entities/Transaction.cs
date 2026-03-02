namespace Barber.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } = default!;
    public string PaymentMethod { get; private set; } = default!;

    private Transaction() { }

    public Transaction(Guid orderId, decimal amount, string type, string paymentMethod)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Type = type;
        PaymentMethod = paymentMethod;
    }
}
