namespace Host.CommunicationProtocols.Order;

public class OrderItem
{
    public int Sku { get; set; }
    public decimal Price { get; set; }
    public byte Quantity { get; set; }
}