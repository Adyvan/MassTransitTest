namespace Models;

public class OrderSagaItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    
    public int Sku { get; set; }
    public decimal Price { get; set; }
    public byte Quantity { get; set; }
}