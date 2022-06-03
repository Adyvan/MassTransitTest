using NHibernate;

namespace Host.Data;

public interface IDbSession
{
    ISessionFactory GetSession();
}
