using Newtonsoft.Json;

namespace Host.CommunicationProtocols.Order;

public class OrderResponse
{
    public long Id { get; set; }
    public long OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerSurname { get; set; }
    
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public DateTime? ShippedDate { get; set; }
    
    public IList<OrderItem> Items { get; set; }

}