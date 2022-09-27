using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public interface ISubtitleCleaner
    {
        public List<byte> DeleteFormatting(byte[] subtitleBytes);
    }
}
