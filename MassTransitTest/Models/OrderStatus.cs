namespace Models;

[Flags]
public enum OrderStatus : byte
{
    Initial = 1,
    AwaitingPacking = 2,
    Packed = 4,
    Cancelled = 8,
    Shipped = 16,
    
    CanCancel = Initial | AwaitingPacking | Packed,
}