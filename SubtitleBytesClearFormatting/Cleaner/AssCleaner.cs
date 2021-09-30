using System;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaner
{
    public class AssCleaner : SubtitleFormatCleaner
    {
        private readonly IReadOnlyCollection<byte> eventTargetBytes;
        private readonly IReadOnlyCollection<byte> formatTargetBytes;
        private readonly IReadOnlyCollection<byte> dialogueTargetBytes;

        public AssCleaner() 
        {
            // Bytes of string: "[Events]"
            eventTargetBytes = new byte[] { 91, 69, 118, 101, 110, 116, 115, 93 };
            // Bytes of string: "Format:"
            formatTargetBytes = new byte[] { 70, 111, 114, 109, 97, 116, 58 };
            // Bytes of string: "Dialogue:"
            dialogueTargetBytes = new byte[] { 68, 105, 97, 108, 111, 103, 117, 101, 58 };
        }

        public override byte[] DeleteFormatting(byte[] subtitleTextBytes)
        {
            if (subtitleTextBytes == null)
                throw new ArgumentNullException(nameof(subtitleTextBytes), "Ass subtitle bytes cannot be null.");

            SubtitleTextBytes = new byte[subtitleTextBytes.Length];
            Array.Copy(subtitleTextBytes, SubtitleTextBytes, subtitleTextBytes.Length);
            TextWithoutFormatting = new List<byte>();

            int eventFormatLength = DetectEventsFormat(out long dialogueStart);
            // If file format doesn't contain '[Events] Format: ,' future algorithm will return empty array
            if (eventFormatLength == 0)
                return Array.Empty<byte>();

            DeleteDialogueFormatting(dialogueStart, eventFormatLength);
            return TextWithoutFormatting.ToArray();
        }

        private int DetectEventsFormat(out long dialogueStart)
        {
            for (dialogueStart = 0; dialogueStart < SubtitleTextBytes.Length; dialogueStart++)
            {
                if (IsEventsWord(dialogueStart, out long shiftPoint) && IsFormatWord(dialogueStart + shiftPoint, ref shiftPoint))
                {
                    dialogueStart += shiftPoint;
                    return EventsFormatLength(ref dialogueStart);
                }
            }

            return 0;
        }

        private bool IsEventsWord(long startPoint, out long shiftPoint)
        {
            shiftPoint = eventTargetBytes.Count;

            if (startPoint + eventTargetBytes.Count >= SubtitleTextBytes.Length)
                return false;

            foreach (byte targetByte in eventTargetBytes)
            {
                if (SubtitleTextBytes[startPoint++] != targetByte)
                    return false;
            }

            if (SubtitleTextBytes[startPoint] == 13)
            {
                if (startPoint + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[startPoint + 1] == 10)
                {
                    shiftPoint += 2;
                    return true;
                }
                shiftPoint++;
                return true;
            }
            if (SubtitleTextBytes[startPoint] == 10)
            {
                shiftPoint++;
                return true;
            }

            return false;
        }

        private bool IsFormatWord(long startPoint, ref long shiftPoint)
        {
            shiftPoint += formatTargetBytes.Count;

            if (startPoint + formatTargetBytes.Count - 1 >= SubtitleTextBytes.Length)
                return false;

            foreach (byte targetByte in formatTargetBytes)
            {
                if (SubtitleTextBytes[startPoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private int EventsFormatLength(ref long dialogueStart)
        {
            int eventFormatLength = 0;
            while (++dialogueStart < SubtitleTextBytes.Length)
            {
                if (SubtitleTextBytes[dialogueStart] == 13)
                {
                    if (dialogueStart + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[dialogueStart] == 10)
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

        private void DeleteDialogueFormatting(long startPoint, int eventFormatLength)
        {
            for (long i = startPoint; i < SubtitleTextBytes.Length; i++)
            {
                if (IsDialogueLine(i))
                {
                    int formatCount = 0;
                    while (++i < SubtitleTextBytes.Length)
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
                                GetDialogueText(ref i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool IsDialogueLine(long startPoint)
        {
            if (startPoint + dialogueTargetBytes.Count - 1 >= SubtitleTextBytes.Length)
                return false;

            foreach (byte targetByte in dialogueTargetBytes)
            {
                if (SubtitleTextBytes[startPoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private void GetDialogueText(ref long startPoint)
        {
            while (++startPoint < SubtitleTextBytes.Length)
            {
                if (SubtitleTextBytes[startPoint] == 13)
                {
                    if (startPoint + 1 < SubtitleTextBytes.Length && SubtitleTextBytes[startPoint + 1] == 10)
                    {
                        TextWithoutFormatting.Add(13);
                        TextWithoutFormatting.Add(10);
                        return;
                    }
                    TextWithoutFormatting.Add(13);
                    return;
                }

                if (SubtitleTextBytes[startPoint] == 10)
                {
                    TextWithoutFormatting.Add(10);
                    return;
                }

                TextWithoutFormatting.Add(SubtitleTextBytes[startPoint]);
            }
        }
    }
}
