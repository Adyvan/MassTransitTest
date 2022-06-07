using Models;
using OrderSaga = Models.OrderSaga;

namespace Host.Services;

public interface IOrderRepositoryService
{
    IList<OrderSaga> GetOrders();
    Task AddOrderAsync(OrderSaga orderSaga);
    Task UpdateOrderStatus(long sagaOrderSagaId, OrderStatus messageNextStatus);
}