using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public abstract class SubtitleFormatCleaner : ISubtitleCleaner, ISubtitleCleanerAsync
    {
        private readonly ReadOnlyCollection<byte> subtitleTextBytes;
        private readonly List<byte> textWithoutFormatting;
        private bool isCleanerWorking;

        protected SubtitleFormatCleaner(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Subtitle bytes cannot be null.");

            this.subtitleTextBytes = Array.AsReadOnly<byte>(subtitleTextBytes);
            textWithoutFormatting = new List<byte>();
            isCleanerWorking = false;
        }

        protected ReadOnlyCollection<byte> SubtitleTextBytes
        {
            get { return subtitleTextBytes; }
        }
        protected List<byte> TextWithoutFormatting
        {
            get { return textWithoutFormatting; }
        }
        protected bool IsCleanerWorking
        {
            get { return isCleanerWorking; }
            set { isCleanerWorking = value; }
        }

        public abstract byte[] DeleteFormatting();
        public virtual async Task<byte[]> DeleteFormattingAsync()
        {
            while (IsCleanerWorking)
                await Task.Delay(50);

            IsCleanerWorking = true;
            byte[] resultBytes = await Task.Run(() => DeleteFormatting());
            IsCleanerWorking = false;

            return resultBytes;
        }

        // Add characters to the textWithoutFormatting list before an empty line
        protected void AddUntilEmptyLine(ref int startpoint)
        {
            while (++startpoint < subtitleTextBytes.Count)
            {
                if (subtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 3 < subtitleTextBytes.Count && subtitleTextBytes[startpoint + 1] == 10
                        && subtitleTextBytes[startpoint + 2] == 13 && subtitleTextBytes[startpoint + 3] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        startpoint += 3;
                        return;
                    }
                    if (startpoint + 1 < subtitleTextBytes.Count && subtitleTextBytes[startpoint + 1] == 13)
                    {
                        TextWithoutFormatting.Add(13);
                        startpoint++;
                        return;
                    }
                }

                if (subtitleTextBytes[startpoint] == 10 && startpoint + 1 < subtitleTextBytes.Count
                    && subtitleTextBytes[startpoint + 1] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    startpoint++;
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startpoint]);
            }
        }
    }
}
