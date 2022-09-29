using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public class SrtCleaner : SubtitleFormatCleaner
    {
        private static readonly IReadOnlyCollection<byte> numberTargetBytes;
        private static readonly IReadOnlyCollection<byte> timingTargetBytes;

        public SrtCleaner() { }

        static SrtCleaner()
        {
            //Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5, 54 = 6, 55 = 7, 56 = 8, 57 = 9
            numberTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
            //Bytes of timing: 32 = ' ', 44 = ,, 58 = :
            timingTargetBytes = new byte[] { 32, 44, 58 };
        }

        /// <summary>
        /// Extracts and returns bytes after srt formatting structures
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing srt subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override List<byte> DeleteFormatting(byte[] subtitleBytes)
        {
            if (subtitleBytes == null)
                throw new ArgumentNullException(nameof(subtitleBytes), "Srt subtitle bytes cannot be null.");
            var deformattedBytes = new List<byte>();

            for (int i = 0; i < subtitleBytes.Length; i++)
            {
                if (IsTimingNumber(subtitleBytes, ref i))
                {
                    if (IsTiming(subtitleBytes, ref i))
                    {
                        AddUntilEmptyLine(subtitleBytes, deformattedBytes, ref i);
                    }
                }
            }

            return deformattedBytes;
        }

        /// <summary>
        /// Extracts and returns bytes after srt formatting structures in async mode
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing srt subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes) =>
            await Task.Run(() => DeleteFormatting(subtitleBytes));

        private bool IsTimingNumber(byte[] initialBytes, ref int startpoint)
        {
            do
            {
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                        startpoint++;
                    return true;
                }
                if (initialBytes[startpoint] == 10)
                    return true;
                if (!numberTargetBytes.Contains(initialBytes[startpoint]))
                    return false;
            } while (startpoint++ < initialBytes.Length);

            return false;
        }

        private bool IsTiming(byte[] initialBytes, ref int startpoint)
        {
            // Bytes timing key: 45 = -, 62 = >
            int timingLineCount = 0;
            int timingPointerCount = 0;

            // Checking timing path
            while (++startpoint < initialBytes.Length)
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
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                        startpoint++;
                    break;
                }
                if (initialBytes[startpoint] == 10)
                    break;

                if (!(numberTargetBytes.Contains(initialBytes[startpoint]) || 
                    timingTargetBytes.Contains(initialBytes[startpoint])))
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
