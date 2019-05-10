namespace EasyCache
{
    public interface ICompressionService
    {
        byte[] Compress(string content);
        string Decompress(byte[] compressed);
    }
}