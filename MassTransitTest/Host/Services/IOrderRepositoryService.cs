using Models;

namespace Host.Services;

public interface IOrderRepositoryService
{
    IList<Order> GetOrders();
    Task AddOrderAsync(Order order);
    Task UpdateOrderStatus(long sagaOrderSagaId, OrderStatus messageNextStatus);
}