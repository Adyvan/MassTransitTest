using Host.Contracts;
using Host.Data;
using Models;
using NHibernate.Linq;
using OrderSaga = Models.OrderSaga;

namespace Host.Services;

public class OrderRepositoryService : IOrderRepositoryService
{
    readonly ILogger<OrderRepositoryService> _logger;
    readonly IDbSession _db;
    readonly ITimeService _time;

    public OrderRepositoryService(IDbSession db, ITimeService time, ILogger<OrderRepositoryService> logger)
    {
        _db = db;
        _time = time;
        _logger = logger;
    }

    public IList<OrderSaga> GetOrders()
    {
        using var db = _db.GetSession();
        using var session = db.OpenSession();

        return session.Query<OrderSaga>()
            .Fetch(x => x.Items)
            .ToList();
    }

    public async Task AddOrderAsync(OrderSaga orderSaga)
    {
        orderSaga.OrderDate = _time.Now;
        orderSaga.UpdatedDate = orderSaga.OrderDate;

        try
        {
            using var db = _db.GetSession();
            using var session = db.OpenSession();
            using var transaction = session.BeginTransaction();

            await session.SaveOrUpdateAsync(orderSaga);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"1 {ex.ToString()}");
            _logger.LogError($"2 {ex.Message}");
            var err = ex;
            while (err.InnerException != null)
            {
                err = err.InnerException;
                _logger.LogError($"3 {err.Message}");
            }
        }
    }

    public async Task UpdateOrderStatus(long sagaOrderSagaId, OrderStatus messageNextStatus)
    {
        try
        {
            using var db = _db.GetSession();
            using var session = db.OpenSession();
            using var transaction = session.BeginTransaction();

            var orderSaga = await session.Query<OrderSaga>().FirstOrDefaultAsync(x => x.Id == sagaOrderSagaId);
            if (orderSaga == null)
            {
                throw new Exception($"Not found Order id {sagaOrderSagaId}");
            }

            orderSaga.UpdatedDate = _time.Now;
            orderSaga.OrderStatus = messageNextStatus;
            
            await session.SaveOrUpdateAsync(orderSaga);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"1 {ex.ToString()}");
            _logger.LogError($"2 {ex.Message}");
            var err = ex;
            while (err.InnerException != null)
            {
                err = err.InnerException;
                _logger.LogError($"3 {err.Message}");
            }
        }
    }
}