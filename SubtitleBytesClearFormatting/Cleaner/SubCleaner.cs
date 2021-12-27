using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class SubCleaner : SubtitleFormatCleaner, ISubtitleCleaner, ISubtitleCleanerAsync
    {
        private static readonly byte[] frameTargetBytes;

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="subtitleTextBytes">Bytes representing sub structures and text after them</param>
        public SubCleaner(byte[] subtitleTextBytes) : base(subtitleTextBytes) { }

        static SubCleaner()
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5, 54 = 6, 55 = 7, 56 = 8, 57 = 9
            frameTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
        }

        /// <summary>
        /// Extracts and returns bytes after sub formatting structures
        /// </summary>
        /// <returns>Returns extracted bytes</returns>
        public override byte[] DeleteFormatting()
        {
            if (TextWithoutFormatting.Count > 0)
                return TextWithoutFormatting.ToArray();

            for (int i = 0; i < SubtitleTextBytes.Count; i++)
            {
                if (IsFrameTiming(ref i))
                {
                    AddTextAfterFrame(ref i);
                }
            }

            return TextWithoutFormatting.ToArray();
        }

        /// <summary>
        /// Extracts and returns bytes after sub formatting structures in async mode
        /// </summary>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<byte[]> DeleteFormattingAsync()
        {
            return await base.DeleteFormattingAsync();
        }

        private bool IsFrameTiming(ref int startpoint)
        {
            // Bytes: 123 = {, 125 = }
            int leftBracketCount = 0;
            int rightBracketCount = 0;

            do
            {
                if (SubtitleTextBytes[startpoint] == 123)
                {
                    leftBracketCount++;
                    continue;
                }
                if (SubtitleTextBytes[startpoint] == 125)
                {
                    rightBracketCount++;
                    if (leftBracketCount == 2 && rightBracketCount == 2)
                        return true;
                    continue;
                }

                if (SubtitleTextBytes[startpoint] == 13)
                    return false;
                if (SubtitleTextBytes[startpoint] == 10)
                    return false;

                if (!frameTargetBytes.Contains(SubtitleTextBytes[startpoint]))
                    return false;
            } while (++startpoint < SubtitleTextBytes.Count);

            return false;
        }

        private void AddTextAfterFrame(ref int startpoint)
        {
            while (++startpoint < SubtitleTextBytes.Count)
            {
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        return;
                    }
                    TextWithoutFormatting.Add(13);
                    return;
                }
                if (SubtitleTextBytes[startpoint] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startpoint]);
            }
        }
    }
}
