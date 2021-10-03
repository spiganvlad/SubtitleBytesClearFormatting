using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace SubtitleBytesClearFormatting.Cleaner
{
    public abstract class SubtitleFormatCleaner : ISubtitleCleaner
    {
        private readonly ReadOnlyCollection<byte> subtitleTextBytes;
        private readonly List<byte> textWithoutFormatting;

        protected SubtitleFormatCleaner(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Subtitle bytes cannot be null.");

            this.subtitleTextBytes = Array.AsReadOnly<byte>(subtitleTextBytes);
            textWithoutFormatting = new List<byte>();
        }

        protected ReadOnlyCollection<byte> SubtitleTextBytes
        {
            get { return subtitleTextBytes; }
        }
        protected List<byte> TextWithoutFormatting
        {
            get { return textWithoutFormatting; }
        }

        public abstract byte[] DeleteFormatting();
        protected abstract void InitializeTargetBytes();

        // Add characters to the list before an empty line
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
