namespace Models;

public class OrderSaga
{
    public virtual long Id { get; set; }
    
    public virtual long OrderNumber { get; set; }
    public virtual DateTime OrderDate { get; set; }
    public virtual DateTime UpdatedDate { get; set; }
    public virtual string CustomerName { get; set; }
    public virtual string CustomerSurname { get; set; }
    public virtual DateTime? ShippedDate { get; set; }

    public virtual IList<OrderSagaItem> Items { get; set; }
    
    public virtual int CurrentState { get; set; }

    public virtual OrderStatus OrderStatus
    {
        get => (OrderStatus)CurrentState;
        set => CurrentState = (int)value;
    }
}