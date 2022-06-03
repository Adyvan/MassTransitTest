using Host.Data;
using Models;
using NHibernate.Linq;

namespace Host.Services;

public class OrderRepositoryService : IOrderRepositoryService
{
    private IDbSession _db;
    private ITimeService _time;

    public OrderRepositoryService(IDbSession db, ITimeService time)
    {
        _db = db;
        _time = time;
    }

    public IList<OrderSaga> GetOrders()
    {
        using var db = _db.GetSession();
        using var session = db.OpenSession();

        return session.Query<OrderSaga>()
            .Fetch(x => x.Items)
            .ToList();
    }
}