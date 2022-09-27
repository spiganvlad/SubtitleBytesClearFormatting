using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.Cleaners
{
    public class AssCleaner : SubtitleFormatCleaner
    {
        private static readonly IReadOnlyCollection<byte> eventTargetBytes;
        private static readonly IReadOnlyCollection<byte> formatTargetBytes;
        private static readonly IReadOnlyCollection<byte> dialogueTargetBytes;

        public AssCleaner() { }

        static AssCleaner()
        {
            // Bytes of string: "[Events]"
            eventTargetBytes = new byte[] { 91, 69, 118, 101, 110, 116, 115, 93 };
            // Bytes of string: "Format:"
            formatTargetBytes = new byte[] { 70, 111, 114, 109, 97, 116, 58 };
            // Bytes of string: "Dialogue:"
            dialogueTargetBytes = new byte[] { 68, 105, 97, 108, 111, 103, 117, 101, 58 };
        }

        /// <summary>
        /// Extracts and returns bytes after ass formatting structures
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing ass subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override List<byte> DeleteFormatting(byte[] subtitleBytes)
        {
            if (subtitleBytes == null)
                throw new ArgumentNullException(nameof(subtitleBytes), "Ass subtitle bytes cannot be null.");
            var deformattedBytes = new List<byte>();

            int eventFormatLength = DetectEventsFormat(subtitleBytes, out int dialogueStart);

            // If file format doesn't contain '[Events] Format: ,' next algorithm will return empty list
            if (eventFormatLength == 0)
                return deformattedBytes;

            DeleteDialogueFormatting(subtitleBytes, deformattedBytes, dialogueStart, eventFormatLength);
            return deformattedBytes;
        }

        /// <summary>
        /// Extracts and returns bytes after ass formatting structures in async mode
        /// </summary>
        /// <param name="subtitleBytes">Bytes representing ass subtitle structures</param>
        /// <returns>Returns extracted bytes</returns>
        public override async Task<List<byte>> DeleteFormattingAsync(byte[] subtitleBytes) =>
            await Task.Run(() => DeleteFormatting(subtitleBytes));
        

        private int DetectEventsFormat(byte[] initialBytes, out int dialogueStart)
        {
            for (dialogueStart = 0; dialogueStart < initialBytes.Length; dialogueStart++)
            {
                if (IsEventsWord(initialBytes, dialogueStart, out int shiftpoint) && IsFormatWord(initialBytes, dialogueStart + shiftpoint, ref shiftpoint))
                {
                    dialogueStart += shiftpoint;
                    return EventsFormatLength(initialBytes, ref dialogueStart);
                }
            }

            return 0;
        }

        private bool IsEventsWord(byte[] initialBytes, int startpoint, out int shiftpoint)
        {
            shiftpoint = eventTargetBytes.Count;

            if (startpoint + eventTargetBytes.Count >= initialBytes.Length)
                return false;

            foreach (byte targetByte in eventTargetBytes)
            {
                if (initialBytes[startpoint++] != targetByte)
                    return false;
            }

            if (initialBytes[startpoint] == 13)
            {
                if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                {
                    shiftpoint += 2;
                    return true;
                }
                shiftpoint++;
                return true;
            }
            if (initialBytes[startpoint] == 10)
            {
                shiftpoint++;
                return true;
            }

            return false;
        }

        private bool IsFormatWord(byte[] initialBytes, int startpoint, ref int shiftpoint)
        {
            shiftpoint += formatTargetBytes.Count;

            if (startpoint + formatTargetBytes.Count - 1 >= initialBytes.Length)
                return false;

            foreach (byte targetByte in formatTargetBytes)
            {
                if (initialBytes[startpoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private int EventsFormatLength(byte[] initialBytes, ref int dialogueStart)
        {
            int eventFormatLength = 0;
            while (++dialogueStart < initialBytes.Length)
            {
                if (initialBytes[dialogueStart] == 13)
                {
                    if (dialogueStart + 1 < initialBytes.Length && initialBytes[dialogueStart] == 10)
                    {
                        dialogueStart += 2;
                        break;
                    }
                    dialogueStart++;
                    break;
                }
                if (initialBytes[dialogueStart] == 10)
                {
                    dialogueStart++;
                    break;
                }
                if (initialBytes[dialogueStart] == 44)
                    eventFormatLength++;
            }

            return eventFormatLength;
        }

        private void DeleteDialogueFormatting(byte[] initialBytes, List<byte> deformattedBytes, int startpoint, int eventFormatLength)
        {
            for (int i = startpoint; i < initialBytes.Length; i++)
            {
                if (IsDialogueLine(initialBytes, i))
                {
                    int formatCount = 0;
                    while (++i < initialBytes.Length)
                    {
                        if (initialBytes[i] == 13)
                            break;
                        if (initialBytes[i] == 10)
                            break;

                        if (initialBytes[i] == 44)
                        {
                            formatCount++;
                            if (formatCount == eventFormatLength)
                            {
                                AddDialogueText(initialBytes, deformattedBytes, ref i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool IsDialogueLine(byte[] initialBytes, int startpoint)
        {
            if (startpoint + dialogueTargetBytes.Count - 1 >= initialBytes.Length)
                return false;

            foreach (byte targetByte in dialogueTargetBytes)
            {
                if (initialBytes[startpoint++] != targetByte)
                    return false;
            }

            return true;
        }

        private void AddDialogueText(byte[] initialBytes, List<byte> deformattedBytes, ref int startpoint)
        {
            while (++startpoint < initialBytes.Length)
            {
                if (initialBytes[startpoint] == 13)
                {
                    if (startpoint + 1 < initialBytes.Length && initialBytes[startpoint + 1] == 10)
                    {
                        deformattedBytes.Add(13);
                        deformattedBytes.Add(10);
                        return;
                    }
                    deformattedBytes.Add(13);
                    return;
                }

                if (initialBytes[startpoint] == 10)
                {
                    deformattedBytes.Add(10);
                    return;
                }

                deformattedBytes.Add(initialBytes[startpoint]);
            }
        }
    }
}