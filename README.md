# SubtitleBytesClearFormatting
.Net library that allows to extract bytes after subtitle formatting structures. Also allows you to remove bytes representing subtitle tags.

Supports extraction from 5 types of subtitles: ass, sbv, srt, sub, vtt.

## Installation
To install, use the [nuget package](https://github.com/spiganvlad?tab=packages&repo_name=SubtitleBytesClearFormatting).

## Cleaner usage
All cleaners work in the same way. They look for subtitle structures and extract the text after them. Let's look at the example of the srt file.

```txt
1
00:00:31,540 --> 00:00:35,190
Hello world!
```

First, we read the bytes from this file. Then we will send them for cleaning. Then we write them to a file and get the following result:

```txt
Hello world!
```

Let's see how to use the cleaners. SrtCleaner was used as an example.

```C#
// First create an instance of the class
SrtCleaner cleaner = new();
// Then you can extract text bytes by passing the subtitle bytes to the method
List<byte> bytesWithoutFormatting = cleaner.DeleteFormatting(subtitleBytes);
// Or do it async
List<byte> bytesWithoutFormatting = await cleaner.DeleteFormattingAsync(subtitleBytes);
```

Static class TxtCleaner allows transforming text bytes.

```C#
// Makes text in one line (Asynchronous version: ToOneLineAsync(byte[] textInBytes))
TxtCleaner.ToOneLine(textInBytes);
// Removes required tags (Asynchronous version: DeleteTagsAsync(byte[] textInBytes, Dictionary<byte, List<TxtTag>> tagsDictionary))
TxtCleaner.DeleteTags(textInBytes, targetTagsDictionary);
```

One abstract class and two interfaces add flexibility to cleaners:
1. SubtitleFormatCleaner - all cleaner inherit from it (except TxtCleaner).
2. ISubtitleCleaner - the inherited must implement the method: List<byte> DeleteFormatting().
3. ISubtitleCleanerAsync - the inherited must implement the method: Task<List<byte>> DeleteFormattingAsync().

## Tags generator usage
Static class TagsCollectionGeneretor allows you to create dictionaries of tags. Consider his methods:
1. GetBasicTags() - return tags: &lt;b&gt;, &lt;/b&gt;, {b}, {/b}, &lt;i&gt;, &lt;/i&gt;, {i}, {/i}, &lt;u&gt;, &lt;/u&gt;, {u}, {/u}, &lt;font*&gt;, &lt;/font&gt;, {\a*}.
2. GetSubSpecificTags() - returns tags: {y:i}, {y:b}, {y:u}, {y:s}, {Y:i}, {Y:b}, {Y:u}, {Y:s}, {f:\*}, {F:\*}, {s:\*}, {S:\*}, {c:\*}, {C:\*}, {p:\*}, {P:\*}, {h:\*}, {H:\*}, {DEFAULT}
3. GetAssSpecificTags() - returns common ass tags.
4. GetTagsFromXml(string path) - returns tags from your XML file.

Dictionaries of tags (Dictionary<byte, List\<TxtTag\>>) have simple structure. Consider his elements:
1. byte - all tags in the list start with this byte.
2. List<TxtTag> - list with target tags.

Class TxtTag represent common tag. Which have a tag name, bytes of that tag and bytes to replace that tag.

## XML tags file
You can use your own XML files to create a dictionary of tags. Just follow the example.

```XML
<tags>
  <tag>
    <!--Name represents the byte to be found in the text (b*, Smile, S*le)-->
    <!--* - means any bytes up to the next character-->
    <!--Don't forget to add a closing tag - {/b*}-->
    <name>{b*}</name>
    <!--replaceBytes - not necessary-->
    <!--Those bytes that will replace the tag-->
    <replaceBytes>
      <byte>58</byte>
      <byte>41</byte>
    </replaceBytes>
  </tag>
  ...
</tags>
```