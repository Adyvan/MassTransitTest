using Host.Contracts;
using MassTransit;

namespace Host.Consumers;

public class AddOrderConsumer :
    IConsumer<AddOrder>
{
    private ILogger<AddOrderConsumer> _logger;

    public AddOrderConsumer(ILogger<AddOrderConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<AddOrder> context)
    {
        _logger.LogInformation($"Consume mess {context.Message.CustomerName}");
        return Task.CompletedTask;
    }
}
