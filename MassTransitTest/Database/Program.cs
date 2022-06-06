using Database.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace Database;

static class Program
{
    static void Main(string[] args)
    {
        CreateDbIfNotExist();

        var serviceProvider = CreateServices();
        
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider);
        }
    }

    private static IServiceProvider CreateServices()
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString("Server=localhost;Database=MassTest;User Id=sa;Password=yourStrong(!)Password123;Encrypt=False;")
                .ScanIn(typeof(OrderTablesMigration).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        
        // Execute the migrations
        runner.MigrateUp();
    }
    static void CreateDbIfNotExist()
    {
        var conn = new SqlConnectionInfo("localhost","sa","yourStrong(!)Password123");
        Server server = new Server(new ServerConnection(conn));
        
        string script = null;
        foreach (var file in Directory.GetFiles("SqlScripts"))
        {
            script = File.ReadAllText(file);
            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}