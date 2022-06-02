using Newtonsoft.Json;

namespace Host.CommunicationProtocols.Order;

public class CreateOrderRequest
{
    public long OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public string CustomerSurname { get; set; }
    
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public DateTime? ShippedDate { get; set; }

    public IList<OrderItem> Items { get; set; }
}