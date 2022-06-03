using Host.Data;
using Host.Services;

namespace Host.Dependency;

public static class Dependencies
{
    public static void SetDependencies(IServiceCollection services)
    {
        services.AddSingleton<IDbSession, DbSession>();
        services.AddSingleton<ITimeService, TimeService>();
    }
}