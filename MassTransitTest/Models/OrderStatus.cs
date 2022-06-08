using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]  
public enum OrderStatus : byte
{
    Initial = 1,
    AwaitingPacking = 2,
    Packed = 4,
    Cancelled = 8,
    Shipped = 16,
    
    CanCustomUpdate = Packed | Cancelled | Shipped,
}