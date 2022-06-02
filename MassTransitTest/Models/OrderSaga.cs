namespace Models;

public class OrderSaga
{
    public long Id { get; set; }
    
    public long OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerSurname { get; set; }
    public DateTime? ShippedDate { get; set; }

    public virtual IList<OrderSagaItem> Items { get; set; }
}