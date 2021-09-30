using System;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class SubCleaner : SubtitleFormatCleaner
    {
        private readonly byte[] frameTargetBytes;

        public SubCleaner() 
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5, 54 = 6, 55 = 7, 56 = 8, 57 = 9
            frameTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
        }

        public override byte[] DeleteFormatting(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Sub subtitle bytes cannot be null.");

            SubtitleTextBytes = new byte[subtitleTextBytes.Length];
            Array.Copy(subtitleTextBytes, SubtitleTextBytes, subtitleTextBytes.Length);
            TextWithoutFormatting = new List<byte>();

            for (long i = 0; i < subtitleTextBytes.Length; i++)
            {
                if (IsFrameTiming(ref i))
                {
                    GetTextAfterFrame(ref i);
                }
            }

            return TextWithoutFormatting.ToArray();
        }

        private bool IsFrameTiming(ref long startPoint)
        {
            // Bytes: 123 = {, 125 = }
            int leftBracketCount = 0;
            int rightBracketCount = 0;

            do
            {
                if (SubtitleTextBytes[startPoint] == 123)
                {
                    leftBracketCount++;
                    continue;
                }
                if (SubtitleTextBytes[startPoint] == 125)
                {
                    rightBracketCount++;
                    if (leftBracketCount == 2 && rightBracketCount == 2)
                        return true;
                    continue;
                }

                if (SubtitleTextBytes[startPoint] == 13)
                    return false;
                if (SubtitleTextBytes[startPoint] == 10)
                    return false;

                if (!frameTargetBytes.Contains(SubtitleTextBytes[startPoint]))
                    return false;
            } while (++startPoint < SubtitleTextBytes.Length);

            return false;
        }

        private void GetTextAfterFrame(ref long startPoint)
        {
            while (++startPoint < SubtitleTextBytes.Length)
            {
                if (SubtitleTextBytes[startPoint] == 13)
                {
                    if (startPoint + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[startPoint + 1] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        return;
                    }
                    TextWithoutFormatting.Add(13);
                    return;
                }
                if (SubtitleTextBytes[startPoint] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startPoint]);
            }
        }
    }
}
