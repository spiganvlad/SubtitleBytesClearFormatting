using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public interface ISubtitleCleanerAsync
    {
        public Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes);
    }
}
