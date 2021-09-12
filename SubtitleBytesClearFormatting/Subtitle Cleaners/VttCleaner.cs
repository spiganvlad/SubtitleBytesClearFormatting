using System;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class VttCleaner : SubtitleFormatCleaner
    {
        private readonly IReadOnlyCollection<byte> timingTargetBytes;

        public VttCleaner() 
        {
            // Bytes of timing: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9, 32 = ' ', 46 = ., 58 = :
            timingTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 32, 46, 58 };
        }

        public override byte[] DeleteFormatting(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Vtt subtitle bytes cannot be null.");

            SubtitleTextBytes = new byte[subtitleTextBytes.Length];
            Array.Copy(subtitleTextBytes, SubtitleTextBytes, subtitleTextBytes.Length);

            for (long i = 0; i < SubtitleTextBytes.Length; i++)
            {
                if (IsTiming(ref i))
                {
                    GetUntilEmptyLine(ref i);
                }
            }

            return TextWithoutFormatting.ToArray();
        }

        private bool IsTiming(ref long startPoint)
        {
            // Bytes of key: 45 = -, 62 = >
            int timingLineCount = 0;
            int timingPointerCount = 0;
            int spaceByteCount = 0;

            // Checking timing path
            do
            {
                if (SubtitleTextBytes[startPoint] == 45)
                {
                    timingLineCount++;
                    continue;
                }
                if (SubtitleTextBytes[startPoint] == 62)
                {
                    timingPointerCount++;
                    continue;
                }
                if (SubtitleTextBytes[startPoint] == 32)
                {
                    spaceByteCount++;
                    if (spaceByteCount > 2)
                    {
                        ScrollToLineEnd(ref startPoint);
                        break;
                    }
                    continue;
                }
                if (SubtitleTextBytes[startPoint] == 13)
                {
                    if (startPoint + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[startPoint + 1] == 10)
                        startPoint++;
                    break;
                }
                if (SubtitleTextBytes[startPoint] == 10)
                    break;

                if (!timingTargetBytes.Contains(SubtitleTextBytes[startPoint]))
                    return false;
            } while (++startPoint < SubtitleTextBytes.Length);

            // Checking key (key = '-' '-' '>' or 45 45 62)
            if (timingLineCount == 2 && timingPointerCount == 1)
            {
                return true;
            }
            return false;
        }

        private void ScrollToLineEnd(ref long startPoint)
        {
            while (++startPoint < SubtitleTextBytes.Length)
            {
                if (SubtitleTextBytes[startPoint] == 10)
                    break;
                if (SubtitleTextBytes[startPoint] == 13)
                {
                    if (startPoint + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[startPoint + 1] == 10)
                        startPoint++;
                    break;
                }
            }
        }
    }
}
