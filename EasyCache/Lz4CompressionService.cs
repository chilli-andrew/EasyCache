using System.IO;
using LZ4;

namespace EasyCache
{
    public class Lz4CompressionService: ICompressionService
    {
        public byte[] Compress(string content)
        {
            var uncompressed = System.Text.Encoding.UTF8.GetBytes(content);
            var output = new MemoryStream();
            using (var stream = new LZ4Stream(output, LZ4StreamMode.Compress))
            {
                stream.Write(uncompressed, 0, uncompressed.Length);
            }

            return output.ToArray();
        }

        public string Decompress(byte[] compressed)
        {
            var input = new MemoryStream(compressed);
            var output = new MemoryStream();
            using (var stream = new LZ4Stream(input, LZ4StreamMode.Decompress))
            {
                stream.CopyTo(output);
            }
            var content = System.Text.Encoding.UTF8.GetString(output.ToArray());
            return content;
        }
    }
}