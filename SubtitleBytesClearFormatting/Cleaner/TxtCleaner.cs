using System;
using System.Collections.Generic;
using SubtitleBytesClearFormatting.TagsGenerate;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public static class TxtCleaner
    {
        public static byte[] ToOneLine(byte[] textInBytes)
        {
            if (textInBytes == null)
                throw new ArgumentNullException(nameof(textInBytes), "Text bytes cannot be null.");

            List<byte> textInOneLine = new();

            for (long i = 0; i < textInBytes.Length; i++)
            {
                if (textInBytes[i] == 13)
                {
                    if (i + 1 < textInBytes.Length && textInBytes[i + 1] == 10)
                        i++;
                    textInOneLine.Add(32);
                }
                else if (textInBytes[i] == 10)
                {
                    textInOneLine.Add(32);
                }
                else
                    textInOneLine.Add(textInBytes[i]);
            }
            
            return textInOneLine.ToArray();
        }

        public static byte[] DeleteTags(byte[] textInBytes, Dictionary<byte, List<TxtTag>> tagsDictionary)
        {
            if (textInBytes == null)
                throw new ArgumentNullException(nameof(textInBytes), "Text bytes cannot be null.");
            if (tagsDictionary == null)
                throw new ArgumentNullException(nameof(tagsDictionary), "Tags dictionary cannot be null.");

            List<byte> textWithoutTags = new();

            for (long i = 0; i < textInBytes.Length; i++)
            {
                if (IsTagFirstByte(textInBytes, tagsDictionary, ref i, out List<TxtTag> tagsList))
                    if (IsTag(textInBytes, i, tagsList, out int tagLength, out List<byte> replaceBytes))
                    {
                        if (replaceBytes != null)
                            textWithoutTags.AddRange(replaceBytes);
                        i += tagLength;
                        continue;
                    }

                textWithoutTags.Add(textInBytes[i]);
            }

            return textWithoutTags.ToArray();
        }

        public static bool IsTagFirstByte(byte[] textInBytes, Dictionary<byte, List<TxtTag>> tagDictionary,
            ref long startpoint, out List<TxtTag> tagsList)
        {
            tagsList = null;

            foreach (byte targetByte in tagDictionary.Keys)
            {
                if (textInBytes[startpoint] == targetByte)
                {
                    tagsList = tagDictionary[targetByte];
                    return true;
                }
            }
            
            return false;
        }

        private static bool IsTag(byte[] textInBytes, long startpoint, List<TxtTag> targetTags,
            out int tagLength, out List<byte> replaceBytes)
        {
            tagLength = 0;
            replaceBytes = null;

            foreach (TxtTag tag in targetTags)
            {
                tagLength = 1;
                for (int i = 1; i < tag.ContainBytes.Count; i++, tagLength++)
                {
                    // Byte 42 = *
                    if (tag.ContainBytes[i] == 42)
                        tagLength += ToNextTagByte(tag.ContainBytes[++i], textInBytes, startpoint + tagLength);
                    else if (textInBytes[startpoint + tagLength] != tag.ContainBytes[i])
                        break;

                    if (i + 1 == tag.ContainBytes.Count)
                    {
                        replaceBytes = tag.ReplaceBytes;
                        return true;
                    }
                }
            }
            
            return false;
        }

        // The method skips bytes to next tag byte
        private static int ToNextTagByte(byte targetByte, byte[] textInBytes, long startpoint)
        {
            int tagSpaceLength = 1;
            while (++startpoint < textInBytes.Length)
            {
                if (textInBytes[startpoint] == targetByte)
                    return tagSpaceLength;
                if (textInBytes[startpoint] == 13)
                    break;
                if (textInBytes[startpoint] == 10)
                    break;
                tagSpaceLength++;
            }

            return 0;
        }
    }
}
