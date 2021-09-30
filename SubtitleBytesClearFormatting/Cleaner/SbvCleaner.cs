using System;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class SbvCleaner : SubtitleFormatCleaner
    {
        private readonly IReadOnlyCollection<byte> numbersTargetBytes;

        public SbvCleaner()
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9,  58 = :, 46 = .
            numbersTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 46 };
        }

        public override byte[] DeleteFormatting(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Sbv subtitle bytes cannot be null.");

            SubtitleTextBytes = new byte[subtitleTextBytes.Length];
            Array.Copy(subtitleTextBytes, SubtitleTextBytes, subtitleTextBytes.Length);
            TextWithoutFormatting = new List<byte>();

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
            // Byte: 44 = ,
            int commaByteCount = 0;
            do
            {
                if (SubtitleTextBytes[startPoint] == 44)
                {
                    commaByteCount++;
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

                if (!numbersTargetBytes.Contains(SubtitleTextBytes[startPoint]))
                    return false;

            } while (++startPoint < SubtitleTextBytes.Length);

            if (commaByteCount == 1)
                return true;

            return false;
        }
    }
}
