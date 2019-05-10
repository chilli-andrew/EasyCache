namespace EasyCache
{
    public class CompressionServiceFactory
    {
        public enum CompressionTypes
        {
            Deflate = 0,
            Lz4 = 1
        }

        public static ICompressionService Create(CompressionTypes compressionType)
        {
            if (compressionType == CompressionTypes.Deflate) return new DeflateCompressionService();
            if (compressionType == CompressionTypes.Lz4) return new Lz4CompressionService();
            return null;
        }
    }
}