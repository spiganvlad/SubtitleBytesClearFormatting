using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class SbvCleaner : SubtitleFormatCleaner, ISubtitleCleaner, ISubtitleCleanerAsync
    {
        private IReadOnlyCollection<byte> numbersTargetBytes;

        public SbvCleaner(byte[] subtitleTextBytes) : base(subtitleTextBytes) { }

        protected override void InitializeTargetBytes()
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9,  58 = :, 46 = .
            numbersTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 46 };
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

        public override async Task<byte[]> DeleteFormattingAsync()
        {
            return await base.DeleteFormattingAsync();
        }

        private bool IsTiming(ref int startpoint)
        {
            // Byte: 44 = ,
            int commaByteCount = 0;
            do
            {
                if (SubtitleTextBytes[startpoint] == 44)
                {
                    commaByteCount++;
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

                if (!numbersTargetBytes.Contains(SubtitleTextBytes[startpoint]))
                    return false;

            } while (++startpoint < SubtitleTextBytes.Count);

            if (commaByteCount == 1)
                return true;

            return false;
        }
    }
}
