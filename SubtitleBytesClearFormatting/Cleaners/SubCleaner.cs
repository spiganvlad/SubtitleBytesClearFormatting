using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public class SubCleaner : SubtitleFormatCleaner
    {
        private static readonly byte[] frameTargetBytes;

        public SubCleaner() { }

        static SubCleaner()
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5, 54 = 6, 55 = 7, 56 = 8, 57 = 9
            frameTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
        }

        /// <summary>
        /// Extracts and returns bytes after sub formatting structures
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing sub subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override List<byte> DeleteFormatting(byte[] subtitleBytes)
        {
            if (subtitleBytes == null)
                throw new ArgumentNullException(nameof(subtitleBytes), "Sub subtitle bytes cannot be null.");
            var deformattedBytes = new List<byte>();

            for (int i = 0; i < subtitleBytes.Length; i++)
            {
                if (IsFrameTiming(subtitleBytes, ref i))
                {
                    AddTextAfterFrame(subtitleBytes, deformattedBytes, ref i);
                }
            }

            return deformattedBytes;
        }

        /// <summary>
        /// Extracts and returns bytes after sub formatting structures in async mode
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing sub subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes) =>
            await Task.Run(() => DeleteFormatting(subtitleBytes));

        private bool IsFrameTiming(byte[] initialBytes, ref int startpoint)
        {
            // Bytes: 123 = {, 125 = }
            int leftBracketCount = 0;
            int rightBracketCount = 0;

            do
            {
                if (initialBytes[startpoint] == 123)
                {
                    leftBracketCount++;
                    continue;
                }
                if (initialBytes[startpoint] == 125)
                {
                    rightBracketCount++;
                    if (leftBracketCount == 2 && rightBracketCount == 2)
                        return true;
                    continue;
                }

                if (initialBytes[startpoint] == 13)
                    return false;
                if (initialBytes[startpoint] == 10)
                    return false;

                if (!frameTargetBytes.Contains(initialBytes[startpoint]))
                    return false;
            } while (++startpoint < initialBytes.Length);

            return false;
        }

        private void AddTextAfterFrame(byte[] initialBytes, List<byte> deformattedBytes, ref int startpoint)
        {
            while (++startpoint < initialBytes.Length)
            {
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                    {
                        deformattedBytes.Add(13);
                        deformattedBytes.Add(10);
                        return;
                    }
                    deformattedBytes.Add(13);
                    return;
                }
                if (initialBytes[startpoint] == 10)
                {
                    deformattedBytes.Add(10);
                    return;
                }

                deformattedBytes.Add(initialBytes[startpoint]);
            }
        }
    }
}