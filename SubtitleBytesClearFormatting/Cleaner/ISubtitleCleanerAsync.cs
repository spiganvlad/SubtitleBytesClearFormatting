using System.Threading.Tasks;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public interface ISubtitleCleanerAsync
    {
        public Task<byte[]> DeleteFormattingAsync();
    }
}
