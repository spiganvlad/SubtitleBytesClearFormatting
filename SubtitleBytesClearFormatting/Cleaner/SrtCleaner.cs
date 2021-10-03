using System;
using System.Linq;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class SrtCleaner : SubtitleFormatCleaner, ISubtitleCleaner
    {
        private IReadOnlyCollection<byte> numberTargetBytes;
        private IReadOnlyCollection<byte> timingTargetBytes;

        public SrtCleaner(byte[] subtitleTextBytes) : base(subtitleTextBytes) { }

        protected override void InitializeTargetBytes()
        {
            //Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5, 54 = 6, 55 = 7, 56 = 8, 57 = 9
            numberTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
            //Bytes of timing: 32 = ' ', 44 = ,, 58 = :
            timingTargetBytes = new byte[] { 32, 44, 58 };
        }

        public override byte[] DeleteFormatting()
        {
            if (TextWithoutFormatting.Count > 0)
                return TextWithoutFormatting.ToArray();

            InitializeTargetBytes();
            for (int i = 0; i < SubtitleTextBytes.Count; i++)
            {
                if (IsTimingNumber(ref i))
                {
                    if (IsTiming(ref i))
                        AddUntilEmptyLine(ref i);
                }
            }

            return TextWithoutFormatting.ToArray();
        }

        private bool IsTimingNumber(ref int startpoint)
        {
            do
            {
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                        startpoint++;
                    return true;
                }
                if (SubtitleTextBytes[startpoint] == 10)
                    return true;
                if (!numberTargetBytes.Contains(SubtitleTextBytes[startpoint]))
                    return false;
            } while (startpoint++ < SubtitleTextBytes.Count);


            return false;
        }

        private bool IsTiming(ref int startpoint)
        {
            // Bytes timing key: 45 = -, 62 = >
            int timingLineCount = 0;
            int timingPointerCount = 0;

            // Checking timing path
            while (++startpoint < SubtitleTextBytes.Count)
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
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
                if (SubtitleTextBytes[startpoint] == 10)
                    break;

                if (!(numberTargetBytes.Contains(SubtitleTextBytes[startpoint]) || 
                    timingTargetBytes.Contains(SubtitleTextBytes[startpoint])))
                    return false;
            }

            // Checking timing key (key = '-' '-' '>' or 45 45 62)
            if (timingLineCount == 2 && timingPointerCount == 1)
            {
                return true;
            }
            return false;
        }
    }
}
