using Host.CommunicationProtocols.Order;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("Orders")]
    public IEnumerable<OrderResponse> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new OrderResponse()
            {
                Id = index,
                OrderDate = DateTime.Now.AddDays(Random.Shared.Next(-20,0)),
                UpdatedDate = DateTime.Now,
                CustomerName = $"Name {index}",
                CustomerSurname = $"Surname {index}",
                ShippedDate = DateTime.Now.AddDays(Random.Shared.Next(20)),
                Items = Enumerable.Range(1, 5).Select(itemIndex => new OrderItem()
                {
                    Sku = Random.Shared.Next(),
                    Price = Random.Shared.Next(10000),
                    Quantity = (byte) Random.Shared.Next(byte.MaxValue),
                }).ToList(),
            })
            .ToArray();
    }
    
    [HttpPost]
    [Route("Create")]
    public OrderResponse Create([FromBody] CreateOrderRequest orderRequest)
    {
        return new OrderResponse()
        {
            Id = Random.Shared.Next(),
            OrderDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            CustomerName = "Name",
            CustomerSurname = "Surname",
            ShippedDate = DateTime.Now.AddDays(Random.Shared.Next(20)),
            Items = orderRequest.Items.Select(item => new OrderItem()
            {
                Sku = item.Sku,
                Price = item.Price,
                Quantity = item.Quantity,
            }).ToList(),
        };
    }
    
    [HttpPut]
    [Route("Update")]

    public OrderResponse Update([FromBody] CreateOrderRequest orderRequest)
    {
        return new OrderResponse()
        {
            Id = Random.Shared.Next(),
            OrderDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            CustomerName = "Name",
            CustomerSurname = "Surname",
            ShippedDate = DateTime.Now.AddDays(Random.Shared.Next(20)),
            Items = orderRequest.Items.Select(item => new OrderItem()
            {
                Sku = item.Sku,
                Price = item.Price,
                Quantity = item.Quantity,
            }).ToList(),
        };
    }
}