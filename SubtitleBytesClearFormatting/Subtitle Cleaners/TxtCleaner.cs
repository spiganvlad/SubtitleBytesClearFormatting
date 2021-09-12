using System;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public static class TxtCleaner
    {
        public static byte[] ToOneLine(byte[] textInBytes)
        {
            if (textInBytes == null)
                throw new ArgumentNullException(nameof(textInBytes), "Text bytes cannot be null.");

            List<byte> textInOneLine = new();

            for (long i = 0; i < textInBytes.Length; i++)
            {
                if (textInBytes[i] == 13)
                {
                    if (i + 1 < textInBytes.Length && textInBytes[i + 1] == 10)
                        i++;
                    textInOneLine.Add(32);
                }
                else if (textInBytes[i] == 10)
                {
                    textInOneLine.Add(32);
                }
                else
                    textInOneLine.Add(textInBytes[i]);
            }
            
            return textInOneLine.ToArray();
        }
    }
}
