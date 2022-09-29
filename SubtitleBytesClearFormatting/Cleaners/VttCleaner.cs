using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public class VttCleaner : SubtitleFormatCleaner
    {
        private static readonly IReadOnlyCollection<byte> timingTargetBytes;

        public VttCleaner() { }

        static VttCleaner()
        {
            // Bytes of timing: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9, 32 = ' ', 46 = ., 58 = :
            timingTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 32, 46, 58 };
        }

        /// <summary>
        /// Extracts and returns bytes after vtt formatting structures
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing vtt subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override List<byte> DeleteFormatting(byte[] subtitleBytes)
        {
            if (subtitleBytes == null)
                throw new ArgumentNullException(nameof(subtitleBytes), "Vtt subtitle bytes cannot be null.");
            var deformattedBytes = new List<byte>();
            
            for (int i = 0; i < subtitleBytes.Length; i++)
            {
                if (IsTiming(subtitleBytes, ref i))
                {
                    AddUntilEmptyLine(subtitleBytes, deformattedBytes, ref i);
                }
            }

            return deformattedBytes;
        }

        /// <summary>
        /// Extracts and returns bytes after vtt formatting structures in async mode
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing vtt subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes) =>
            await Task.Run(() => DeleteFormatting(subtitleBytes));

        private bool IsTiming(byte[] initialBytes, ref int startpoint)
        {
            // Bytes of key: 45 = -, 62 = >
            int timingLineCount = 0;
            int timingPointerCount = 0;
            int spaceByteCount = 0;

            // Checking timing path
            do
            {
                if (initialBytes[startpoint] == 45)
                {
                    timingLineCount++;
                    continue;
                }
                if (initialBytes[startpoint] == 62)
                {
                    timingPointerCount++;
                    continue;
                }
                if (initialBytes[startpoint] == 32)
                {
                    spaceByteCount++;
                    if (spaceByteCount > 2)
                    {
                        ScrollToLineEnd(initialBytes, ref startpoint);
                        break;
                    }
                    continue;
                }
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
                if (initialBytes[startpoint] == 10)
                    break;

                if (!timingTargetBytes.Contains(initialBytes[startpoint]))
                    return false;
            } while (++startpoint < initialBytes.Length);

            // Checking key (key = '-' '-' '>' or 45 45 62)
            if (timingLineCount == 2 && timingPointerCount == 1)
            {
                return true;
            }

            return false;
        }

        private void ScrollToLineEnd(byte[] initialBytes, ref int startpoint)
        {
            while (++startpoint < initialBytes.Length)
            {
                if (initialBytes[startpoint] == 10)
                    break;
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
            }
        }
    }
}