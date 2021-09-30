using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SubtitleBytesClearFormatting.TagsGenerate
{
    public static class TagsCollectionGeneretor
    {
        public static Dictionary<byte, List<TxtTag>> GetBasicTags()
        {   
            string path = Directory.GetCurrentDirectory() + @"\SubtitleTags\BasicTags.xml";
            if (File.Exists(path))
                return ExtractTagsFromXml(path);

            throw new FileNotFoundException("File BasicTags.xml not found.", path);
        }

        public static Dictionary<byte, List<TxtTag>> GetSubSpecificTags()
        {
            string path = Directory.GetCurrentDirectory() + @"\SubtitleTags\SubTags.xml";
            if (File.Exists(path))
                return ExtractTagsFromXml(path);

            throw new FileNotFoundException("File SubTags.xml not found.", path);
        }

        public static Dictionary<byte, List<TxtTag>> GetAssSpecificTags()
        {   
            string path = Directory.GetCurrentDirectory() + @"\SubtitleTags\AssTags.xml";
            if (File.Exists(path))
                return ExtractTagsFromXml(path);

            throw new FileNotFoundException("File AssTags.xml not found.", path);
        }

        public static Dictionary<byte, List<TxtTag>> GetTagsFromXml(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or white space.", nameof(path));

            if (File.Exists(path))
                return ExtractTagsFromXml(path);

            throw new FileNotFoundException("Xml file not found.", path);
        }

        private static Dictionary<byte, List<TxtTag>> ExtractTagsFromXml(string path)
        {
            Dictionary<byte, List<TxtTag>> tagGroup = new();
            XDocument xDoc = XDocument.Load(path);

            if (xDoc.Element("tags") == null)
                throw new XmlException("Xml root \"tags\" not found.");

            if (!xDoc.Element("tags").Elements("tag").Any())
                throw new XmlException("Xml root \"tags\" not contain \"tag\" root.");

            foreach (XElement xElem in xDoc.Element("tags").Elements("tag"))
            {
                TxtTag tempTag = new();

                if (String.IsNullOrWhiteSpace(xElem.Element("name").Value))
                    throw new XmlException("Xml tag name value cannot be null or white space.");

                tempTag.Name = xElem.Element("name").Value;
                tempTag.ContainBytes = new List<byte>(Encoding.UTF8.GetBytes(tempTag.Name));

                if (xElem.Element("replace") != null)
                    tempTag.ReplaceBytes = new List<byte>(Encoding.UTF8.GetBytes(xElem.Element("replace").Value));
                else if (xElem.Element("replaceBytes") != null)
                    tempTag.ReplaceBytes = GetReplaceByteList(xElem.Element("replaceBytes"));

                if (tagGroup.ContainsKey(tempTag.ContainBytes[0]))
                    tagGroup[tempTag.ContainBytes[0]].Add(tempTag);
                else
                    tagGroup.Add(tempTag.ContainBytes[0], new List<TxtTag> { tempTag });
            }

            return tagGroup;
        }

        private static List<byte> GetReplaceByteList(XElement RepleceElem)
        {
            List<byte> replaceList = new();

            if (!RepleceElem.Elements("byte").Any())
                throw new XmlException("Xml tag \"byte\" now found in root \"replaceBytes\".");

            foreach (XElement xElem in RepleceElem.Elements("byte"))
            {
                if (Byte.TryParse(xElem.Value, out byte tempByte))
                    replaceList.Add(tempByte);
                else
                    throw new XmlException("Xml tag \"byte\" contains a non byte value.");
            }

            return replaceList;
        }
    }
}
