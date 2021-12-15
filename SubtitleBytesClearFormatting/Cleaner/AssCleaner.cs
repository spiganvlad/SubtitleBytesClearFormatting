using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class AssCleaner : SubtitleFormatCleaner, ISubtitleCleaner, ISubtitleCleanerAsync
    {
        private static readonly IReadOnlyCollection<byte> eventTargetBytes;
        private static readonly IReadOnlyCollection<byte> formatTargetBytes;
        private static readonly IReadOnlyCollection<byte> dialogueTargetBytes;

        public AssCleaner(byte[] subtitleTextBytes) : base(subtitleTextBytes) { }

        static AssCleaner()
        {
            // Bytes of string: "[Events]"
            eventTargetBytes = new byte[] { 91, 69, 118, 101, 110, 116, 115, 93 };
            // Bytes of string: "Format:"
            formatTargetBytes = new byte[] { 70, 111, 114, 109, 97, 116, 58 };
            // Bytes of string: "Dialogue:"
            dialogueTargetBytes = new byte[] { 68, 105, 97, 108, 111, 103, 117, 101, 58 };
        }

        public override byte[] DeleteFormatting()
        {
            if (TextWithoutFormatting.Count > 0)
                return TextWithoutFormatting.ToArray();

            int eventFormatLength = DetectEventsFormat(out int dialogueStart);

            // If file format doesn't contain '[Events] Format: ,' next algorithm will return empty array
            if (eventFormatLength == 0)
                return Array.Empty<byte>();

            DeleteDialogueFormatting(dialogueStart, eventFormatLength);
            return TextWithoutFormatting.ToArray();
        }

        public override async Task<byte[]> DeleteFormattingAsync()
        {
            return await base.DeleteFormattingAsync();
        }

        private int DetectEventsFormat(out int dialogueStart)
        {
            for (dialogueStart = 0; dialogueStart < SubtitleTextBytes.Count; dialogueStart++)
            {
                if (IsEventsWord(dialogueStart, out int shiftpoint) && IsFormatWord(dialogueStart + shiftpoint, ref shiftpoint))
                {
                    dialogueStart += shiftpoint;
                    return EventsFormatLength(ref dialogueStart);
                }
            }

            return 0;
        }

        private bool IsEventsWord(int startpoint, out int shiftpoint)
        {
            shiftpoint = eventTargetBytes.Count;

            if (startpoint + eventTargetBytes.Count >= SubtitleTextBytes.Count)
                return false;

            foreach (byte targetByte in eventTargetBytes)
            {
                if (SubtitleTextBytes[startpoint++] != targetByte)
                    return false;
            }

            if (SubtitleTextBytes[startpoint] == 13)
            {
                if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                {
                    shiftpoint += 2;
                    return true;
                }
                shiftpoint++;
                return true;
            }
            if (SubtitleTextBytes[startpoint] == 10)
            {
                shiftpoint++;
                return true;
            }

            return false;
        }

        private bool IsFormatWord(int startpoint, ref int shiftpoint)
        {
            shiftpoint += formatTargetBytes.Count;

            if (startpoint + formatTargetBytes.Count - 1 >= SubtitleTextBytes.Count)
                return false;

            foreach (byte targetByte in formatTargetBytes)
            {
                if (SubtitleTextBytes[startpoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private int EventsFormatLength(ref int dialogueStart)
        {
            int eventFormatLength = 0;
            while (++dialogueStart < SubtitleTextBytes.Count)
            {
                if (SubtitleTextBytes[dialogueStart] == 13)
                {
                    if (dialogueStart + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[dialogueStart] == 10)
                    {
                        dialogueStart += 2;
                        break;
                    }
                    dialogueStart++;
                    break;
                }
                if (SubtitleTextBytes[dialogueStart] == 10)
                {
                    dialogueStart++;
                    break;
                }
                if (SubtitleTextBytes[dialogueStart] == 44)
                    eventFormatLength++;
            }

            return eventFormatLength;
        }

        private void DeleteDialogueFormatting(int startpoint, int eventFormatLength)
        {
            for (int i = startpoint; i < SubtitleTextBytes.Count; i++)
            {
                if (IsDialogueLine(i))
                {
                    int formatCount = 0;
                    while (++i < SubtitleTextBytes.Count)
                    {
                        if (SubtitleTextBytes[i] == 13)
                            break;
                        if (SubtitleTextBytes[i] == 10)
                            break;

                        if (SubtitleTextBytes[i] == 44)
                        {
                            formatCount++;
                            if (formatCount == eventFormatLength)
                            {
                                AddDialogueText(ref i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool IsDialogueLine(int startpoint)
        {
            if (startpoint + dialogueTargetBytes.Count - 1 >= SubtitleTextBytes.Count)
                return false;

            foreach (byte targetByte in dialogueTargetBytes)
            {
                if (SubtitleTextBytes[startpoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private void AddDialogueText(ref int startpoint)
        {
            while (++startpoint < SubtitleTextBytes.Count)
            {
                if (SubtitleTextBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < SubtitleTextBytes.Count && SubtitleTextBytes[startpoint + 1] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        return;
                    }
                    TextWithoutFormatting.Add(13);
                    return;
                }

                if (SubtitleTextBytes[startpoint] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startpoint]);
            }
        }
    }
}
