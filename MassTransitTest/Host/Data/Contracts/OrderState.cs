namespace Host.Contracts;

public record OrderState
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
}