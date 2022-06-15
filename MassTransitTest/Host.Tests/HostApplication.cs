using Host.Data;
using Host.Tests.Db;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Host.Tests;

public class HostApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // builder.ConfigureServices(services =>
        // {
        //     services.RemoveAll(typeof(DbSession));
        //     services.AddScoped<IDbSession, TestDbSession>();
        // });

        return base.CreateHost(builder);
    }
}
