namespace Host.Helpers;

public static class IdentifierExtensions
{
    public static Guid ToGuid(this long id)
    {
        byte[] data = new byte[16];
        Array.Copy(BitConverter.GetBytes(id), 0, data, 0, 8);
        return new Guid(data);
    }
    
    /// <summary>
    /// We are lost first 8 byte 
    /// </summary>
    /// <param name="guid"></param>
    /// <returns>long</returns>
    public static long ToId(this Guid guid)
    {
        var array = guid.ToByteArray();
        var l = BitConverter.ToInt64(array, 0);
        return l;
    }
}