namespace Host.Contracts;

public record UpdateOrder(Guid CorrelationId, int NewState);