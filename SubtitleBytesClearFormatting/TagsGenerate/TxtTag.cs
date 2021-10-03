using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.TagsGenerate
{
    public class TxtTag
    {
        public string Name { get; set; }

        // The sequence of bytes to look for in the subtitles
        public List<byte> ContainBytes { get; set; }

        // The sequence of bytes to be inserted in place of the tag 
        public List<byte> ReplaceBytes { get; set; }

        public TxtTag() { }
    }
}
