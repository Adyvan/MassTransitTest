using Host.CommunicationProtocols.Order;

namespace Host.Contracts;

public record AddOrder
{
    public string CustomerName { get; init; }
    public string CustomerSurname { get; init; }
    public DateTime? ShippedDate { get; init; }

    public virtual IList<OrderItem> Items { get; init; }
}
