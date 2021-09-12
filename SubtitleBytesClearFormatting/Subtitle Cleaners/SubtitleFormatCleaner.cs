using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public abstract class SubtitleFormatCleaner
    {
        private byte[] subtitleTextBytes;
        private List<byte> textWithoutFormatting;

        protected SubtitleFormatCleaner() 
        {
            TextWithoutFormatting = new List<byte>();
        }

        protected byte[] SubtitleTextBytes
        {
            get { return subtitleTextBytes; }
            set { subtitleTextBytes = value; }
        }
        protected List<byte> TextWithoutFormatting
        {
            get { return textWithoutFormatting; }
            set { textWithoutFormatting = value; }
        }

        public abstract byte[] DeleteFormatting(byte[] subtitleTextBytes);

        // Add characters to the list before an empty line
        protected void GetUntilEmptyLine(ref long startPoint)
        {
            while (++startPoint < subtitleTextBytes.Length)
            {
                if (subtitleTextBytes[startPoint] == 13)
                {
                    if (startPoint + 3 < subtitleTextBytes.Length && subtitleTextBytes[startPoint + 1] == 10
                        && subtitleTextBytes[startPoint + 2] == 13 && subtitleTextBytes[startPoint + 3] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        startPoint += 3;
                        return;
                    }
                    if (startPoint + 1 < subtitleTextBytes.Length && subtitleTextBytes[startPoint + 1] == 13)
                    {
                        TextWithoutFormatting.Add(13);
                        startPoint++;
                        return;
                    }
                }

                if (subtitleTextBytes[startPoint] == 10 && startPoint + 1 < subtitleTextBytes.Length
                    && subtitleTextBytes[startPoint + 1] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    startPoint++;
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startPoint]);
            }
        }
    }
}
