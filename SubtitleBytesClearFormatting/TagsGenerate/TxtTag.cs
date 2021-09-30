using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.TagsGenerate
{
    public class TxtTag
    {
        public string Name { get; set; }
        public List<byte> ContainBytes { get; set; }
        public List<byte> ReplaceBytes { get; set; }

        public TxtTag() { }
    }
}
