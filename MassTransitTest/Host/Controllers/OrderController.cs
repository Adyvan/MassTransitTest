using Host.CommunicationProtocols.Order;
using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Host.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    readonly ILogger<OrderController> _logger;
    readonly IBus _bus;
    private readonly IOrderRepositoryService _repository;

    public OrderController(ILogger<OrderController> logger, IBus bus, IOrderRepositoryService repository)
    {
        _logger = logger;
        _bus = bus;
        _repository = repository;
    }

    [HttpGet]
    [Route("Orders")]
    public IEnumerable<OrderResponse> Get()
    {
        return _repository.GetOrders().Select(order =>
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UpdatedDate = order.UpdatedDate,
                CustomerName = order.CustomerName,
                CustomerSurname = order.CustomerSurname,
                ShippedDate = order.ShippedDate,
                OrderStatus = order.OrderStatus,
                Items = order.Items.Select(item => new OrderItem()
                {
                    Sku = item.Sku,
                    Price = item.Price,
                    Quantity = item.Quantity,
                }).ToList(),
            };
        });
    }
    
    [HttpPost]
    [Route("Create")]
    public async Task<OkResult> Create([FromBody] CreateOrderRequest orderRequest)
    {
        await _bus.Publish(new AddOrder
            {
                    CustomerName = orderRequest.CustomerName,
                    CustomerSurname = orderRequest.CustomerSurname,
                    ShippedDate = orderRequest.ShippedDate,
                    Items = orderRequest.Items.Select(item => new OrderItem()
                    {
                        Sku = item.Sku,
                        Price = item.Price,
                        Quantity = item.Quantity,
                    }).ToList()
            });
        return new OkResult();
    }
    
    [HttpPut]
    [Route("{id:long}/UpdateStatus")]
    public async Task<ActionResult> Update([FromRoute] long id, [FromQuery] OrderStatus newState)
    {
        await _bus.Publish(new OrderStatusChanged(id.ToGuid(), newState));
        return new OkResult();
    }
}