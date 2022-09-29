using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public abstract class SubtitleFormatCleaner : ISubtitleCleaner, ISubtitleCleanerAsync
    {
        protected SubtitleFormatCleaner() { }

        public abstract List<byte> DeleteFormatting(byte[] subtitleBytes);
        public abstract Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes);

        // Add bytes to the list before an empty line
        protected void AddUntilEmptyLine(byte[] initialBytes, List<byte> deformattedBytes, ref int startpoint)
        {
            while (++startpoint < initialBytes.Length)
            {
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 3 < initialBytes.Length && initialBytes[startpoint + 1] == 10
                        && initialBytes[startpoint + 2] == 13 && initialBytes[startpoint + 3] == 10)
                    {
                        deformattedBytes.Add(13);
                        deformattedBytes.Add(10);
                        startpoint += 3;
                        return;
                    }

                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 13)
                    {
                        deformattedBytes.Add(13);
                        startpoint++;
                        return;
                    }
                }

                if (initialBytes[startpoint] == 10 && startpoint + 1 < initialBytes.Length
                    && initialBytes[startpoint + 1] == 10)
                {
                    deformattedBytes.Add(10);
                    startpoint++;
                    return;
                }

                deformattedBytes.Add(initialBytes[startpoint]);
            }
        }
    }
}
