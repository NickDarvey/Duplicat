using System.Collections.Generic;

namespace System.IO
{
    internal static class StreamExtensions
    {
        public static IEnumerable<byte> AsEnumerable(this Stream stream)
        {
            if (stream != null)
            {
                for (int i = stream.ReadByte(); i != -1; i = stream.ReadByte())
                    yield return (byte)i;
            }
        }
    }
}
