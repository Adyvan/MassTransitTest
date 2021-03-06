namespace Models;

public class OrderItem
{
    public virtual long Id { get; set; }
    public virtual long OrderId { get; set; }

    public virtual int Sku { get; set; }
    public virtual decimal Price { get; set; }
    public virtual byte Quantity { get; set; }

    public virtual Order Order { get; set; }
}