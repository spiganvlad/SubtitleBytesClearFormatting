using System;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class VttCleaner : SubtitleFormatCleaner, ISubtitleCleaner
    {
        private IReadOnlyCollection<byte> timingTargetBytes;

        public VttCleaner(byte[] subtitleTextBytes) : base(subtitleTextBytes) { }

        protected override void InitializeTargetBytes()
        {
            // Bytes of timing: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9, 32 = ' ', 46 = ., 58 = :
            timingTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 32, 46, 58 };
        }

        public override byte[] DeleteFormatting()
        {
            if (TextWithoutFormatting.Count > 0)
                return TextWithoutFormatting.ToArray();

            InitializeTargetBytes();
            for (int i = 0; i < SubtitleTextBytes.Count; i++)
            {
                if (IsTiming(ref i))
                {
                    AddUntilEmptyLine(ref i);
                }
            }

            return TextWithoutFormatting.ToArray();
        }

        private bool IsTiming(ref int startpoint)
        {
            // Bytes of key: 45 = -, 62 = >
            int timingLineCount = 0;
            int timingPointerCount = 0;
            int spaceByteCount = 0;

            // Checking timing path
            do
            {
                if (SubtitleTextBytes[startpoint] == 45)
                {
                    timingLineCount++;
                    continue;
                }
                if (SubtitleTextBytes[startpoint] == 62)
                {
                    timingPointerCount++;
                    continue;
                }
                if (SubtitleTextBytes[startpoint] == 32)
                {
                    spaceByteCount++;
                    if (spaceByteCount > 2)
                    {
                        ScrollToLineEnd(ref startpoint);
                        break;
                    }
                    continue;
                }
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
                if (SubtitleTextBytes[startpoint] == 10)
                    break;

                if (!timingTargetBytes.Contains(SubtitleTextBytes[startpoint]))
                    return false;
            } while (++startpoint < SubtitleTextBytes.Count);

            // Checking key (key = '-' '-' '>' or 45 45 62)
            if (timingLineCount == 2 && timingPointerCount == 1)
            {
                return true;
            }
            return false;
        }

        private void ScrollToLineEnd(ref int startpoint)
        {
            while (++startpoint < SubtitleTextBytes.Count)
            {
                if (SubtitleTextBytes[startpoint] == 10)
                    break;
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
            }
        }
    }
}
