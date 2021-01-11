namespace Redox.DataModel.Builder.Extensions
{
    using System.IO;

    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream is MemoryStream memStream)
            {
                return memStream.ToArray();
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
