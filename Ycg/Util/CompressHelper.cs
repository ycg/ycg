using System;
using System.IO;
using System.IO.Compression;
using System.Text;

/*
 *  
 *       2014-1-29
 *          1. 完成对压缩帮助类代码的编写
 *          2. 出现一个问题： - “块的长度与它的补集不匹配” → Deflate
 *          3. 还出现一个问题： - “内存流不可以扩展” → GZip
 * 
 */

namespace Ycg.Util
{
    public sealed class CompressHelper
    {
        #region GZip Compress Or Decompress

        public static string GZipCompress(string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue)) return string.Empty;
            byte[] sourceBytes = Encoding.UTF8.GetBytes(sourceValue);
            byte[] compressBytes = GZipCompress(sourceBytes);
            return Convert.ToBase64String(compressBytes);
        }

        public static string GZipDecompress(string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue)) return string.Empty;
            byte[] sourceBytes = Convert.FromBase64String(sourceValue);
            return Encoding.UTF8.GetString(GZipDecompress(sourceBytes));
        }

        public static byte[] GZipCompress(byte[] sourceBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(sourceBytes))
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(sourceBytes, 0, sourceBytes.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] GZipDecompress(byte[] souceBytes)
        {
            return Decompress(souceBytes, CompressType.GZip);
        }

        private static byte[] Decompress(byte[] souceBytes, CompressType compressType)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (MemoryStream inStream = new MemoryStream(souceBytes))
                {
                    using (Stream stream = GetCompressStream(compressType, inStream))
                    {
                        return ReadAllBytesFromStream(stream, outStream);
                    }
                }
            }
        }

        private static Stream GetCompressStream(CompressType compressType, MemoryStream inStream)
        {
            Stream stream = null;
            switch (compressType)
            {
                case CompressType.GZip:
                    stream = new GZipStream(inStream, CompressionMode.Decompress, true);
                    break;
                case CompressType.Deflate:
                    stream = new DeflateStream(inStream, CompressionMode.Decompress, true);
                    break;
            }
            return stream;
        }

        private static byte[] ReadAllBytesFromStream(Stream compressStream, MemoryStream outStream)
        {
            int readLength = 0;
            byte[] readBytes = new byte[1024];
            while ((readLength = compressStream.Read(readBytes, 0, readBytes.Length)) > 0)
            {
                outStream.Write(readBytes, 0, readLength);
            }
            return outStream.ToArray();
        }

        #endregion

        #region Deflate Compress Or Decompress

        public static string DeflateCompress(string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue)) return string.Empty;
            byte[] sourceBytes = Encoding.UTF8.GetBytes(sourceValue);
            byte[] compressBytes = DeflateCompress(sourceBytes);
            return Convert.ToBase64String(compressBytes);
        }

        public static byte[] DeflateCompress(byte[] sourceBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(sourceBytes))
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
                {
                    deflateStream.Write(sourceBytes, 0, sourceBytes.Length);
                    return memoryStream.ToArray();
                }
            }
        }

        public static string DeflateDecompress(string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue)) return string.Empty;
            byte[] sourceBytes = Convert.FromBase64String(sourceValue);
            return Encoding.UTF8.GetString(DeflateDecompress(sourceBytes));
        }

        public static byte[] DeflateDecompress(byte[] souceBytes)
        {
            return Decompress(souceBytes, CompressType.Deflate);
        }

        #endregion
    }

    public enum CompressType
    {
        GZip = 0,
        Deflate = 1
    }
}