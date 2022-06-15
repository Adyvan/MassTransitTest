﻿using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Host.Data;
using Host.Data.Mapping;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Host.Tests.Db;

public class TestDbSession : IDbSession
{
    public ISessionFactory GetSession()
    {
        return Fluently.Configure()
            .Database(
                MsSqlConfiguration.MsSql2012.ConnectionString("Server=localhost;Database=MassTest;User Id=sa;Password=yourStrong(!)Password123;"))
            .Mappings(m =>
            {
                m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(OrderSagaMap)));
                // m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(Order)));
            })
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration config)
    {
        new SchemaUpdate(config)
            .Execute(true, true);
    }
    // public ISessionFactory GetSession()
    // {
    //     return Fluently.Configure()
    //         .Database(SQLiteConfiguration.Standard.InMemory())
    //         .Mappings(m =>
    //         {
    //             m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(OrderSagaMap)));
    //             // m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(Order)));
    //         })
    //         .ExposeConfiguration(BuildSchema)
    //         .BuildSessionFactory();
    // }
    //
    // private static void BuildSchema(Configuration config)
    // {
    //     new SchemaUpdate(config)
    //         .Execute(true, true);
    // }
}