using Models;

namespace Host.Services;

public interface IOrderRepositoryService
{
    IList<OrderSaga> GetOrders();
}