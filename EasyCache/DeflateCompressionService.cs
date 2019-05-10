using System.IO;
using System.IO.Compression;

namespace EasyCache
{
    public class DeflateCompressionService: ICompressionService
    {
        public byte[] Compress(string content)
        {
            var uncompressed = System.Text.Encoding.UTF8.GetBytes(content);
            var output = new MemoryStream();
            using (var stream = new DeflateStream(output, CompressionLevel.Fastest))
            {
                stream.Write(uncompressed, 0, uncompressed.Length);
            }

            return output.ToArray();
        }

        public string Decompress(byte[] compressed)
        {
            var input = new MemoryStream(compressed);
            var output = new MemoryStream();
            using (var stream = new DeflateStream(input, CompressionMode.Decompress))
            {
                stream.CopyTo(output);
            }
            var content = System.Text.Encoding.UTF8.GetString(output.ToArray());
            return content;
        }
    }
}