namespace Host.Services;

public class TimeService : ITimeService
{
    public DateTime Now => DateTime.UtcNow;
}