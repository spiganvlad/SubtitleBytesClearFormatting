using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormattingTest.Helpers
{
    public static class BytesLoadHelper
    {
        public static byte[] GetFileBytesArray(string path)
        {
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            byte[] fileBytes = new byte[fs.Length];
            fs.Read(fileBytes, 0, fileBytes.Length);
            return fileBytes;
        }

        public static List<byte> GetFileBytesList(string path) => GetFileBytesArray(path).ToList();
    }
}
