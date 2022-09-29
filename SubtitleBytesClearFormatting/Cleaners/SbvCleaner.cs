using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public class SbvCleaner : SubtitleFormatCleaner
    {
        private static readonly IReadOnlyCollection<byte> numbersTargetBytes;

        public SbvCleaner() { }

        static SbvCleaner()
        {
            // Bytes of numbers: 48 = 0, 49 = 1, 50 = 2, 51 = 3, 52 = 4, 53 = 5,
            // 54 = 6, 55 = 7, 56 = 8, 57 = 9,  58 = :, 46 = .
            numbersTargetBytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 46 };
        }

        /// <summary>
        /// Extracts and returns bytes after sbv formatting structures
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing sbv subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override List<byte> DeleteFormatting(byte[] subtitleBytes)
        {
            if (subtitleBytes == null)
                throw new ArgumentNullException(nameof(subtitleBytes), "Sbv subtitle bytes cannot be null.");
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
        /// Extracts and returns bytes after sbv formatting structures in async mode
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing sbv subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes) =>
            await Task.Run(() => DeleteFormatting(subtitleBytes));

        private bool IsTiming(byte[] initialBytes, ref int startpoint)
        {
            // Byte: 44 = ,
            int commaByteCount = 0;
            do
            {
                if (initialBytes[startpoint] == 44)
                {
                    commaByteCount++;
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

                if (!numbersTargetBytes.Contains(initialBytes[startpoint]))
                    return false;

            } while (++startpoint < initialBytes.Length);

            if (commaByteCount == 1)
                return true;

            return false;
        }
    }
}