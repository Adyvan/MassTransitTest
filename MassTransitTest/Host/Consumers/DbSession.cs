using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Host.Data.Mapping;
using Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Host.Data;

public class DbSession : IDbSession
{
    public ISessionFactory GetSession()
    {
        return Fluently.Configure()
            .Database(MsSqliteConfiguration.Standard.ConnectionString(
                "Server=localhost;Database=MassTest;User Id=sa;Password=yourStrong(!)Password123;"))
            .Mappings(m =>
            {
                m.FluentMappings.Add<OrderSagaMap>();
                m.FluentMappings.Add<OrderSagaItem>();
            })
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    }
    
    private static void BuildSchema(Configuration config)
    {
        // this NHibernate tool takes a configuration (with mapping info in)
        // and exports a database schema from it
        new SchemaExport(config)
            .Create(false, true);
    }
}