using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormatting.TagsGenerate;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesClearFormattingTest
{
    public class TxtCleanerTests
    {
        [Fact]
        public void ToOneLineNullParameter()
        {
            byte[] textBytes = null;

            void act() => TxtCleaner.ToOneLine(textBytes);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Text bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void ToOneLineEmptyParameter()
        {
            byte[] textBytes = Array.Empty<byte>();

            byte[] resultBytes = TxtCleaner.ToOneLine(textBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void ToOneLineReturnCorrectWindowsValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/ToOneLineWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/ToOneLineCasesResult.txt");

            byte[] resultBytes = TxtCleaner.ToOneLine(txtBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void ToOneLineReturnCorrectUnixValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/ToOneLineUnixCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/ToOneLineCasesResult.txt");

            byte[] resultBytes = TxtCleaner.ToOneLine(txtBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void ToOneLineReturnCorrectMacintoshValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/ToOneLineMacintoshCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/ToOneLineCasesResult.txt");

            byte[] resultBytes = TxtCleaner.ToOneLine(txtBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task ToOneLineAsyncReturnCorrectValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/ToOneLineWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/ToOneLineCasesResult.txt");

            byte[] resultBytes = await TxtCleaner.ToOneLineAsync(txtBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsNullArrayParameter()
        {
            byte[] textBytes = null;
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetBasicTags();

            void act() => TxtCleaner.DeleteTags(textBytes, tags);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Text bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteTagsEmptyArrayParameter()
        {
            byte[] textBytes = Array.Empty<byte>();
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetBasicTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(textBytes, tags);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteTagsNullDictionaryParameter()
        {
            byte[] textBytes = { 1 };
            Dictionary<byte, List<TxtTag>> tags = null;

            void act() => TxtCleaner.DeleteTags(textBytes, tags);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Tags dictionary cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteTagsEmptyDictionaryParameter()
        {
            byte[] textBytes = { 1 };
            Dictionary<byte, List<TxtTag>> tags = new();

            byte[] resultBytes = TxtCleaner.DeleteTags(textBytes, tags);

            Assert.Equal(textBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectAssWindowsValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsAssWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsAssWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetAssSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectAssUnixValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsAssUnixCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsAssUnixCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetAssSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectAssMacintoshValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsAssMacintoshCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsAssMacintoshCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetAssSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectSubWindowsValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsSubWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsSubWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetSubSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectSubUnixValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsSubUnixCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsSubUnixCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetSubSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectSubMacintoshValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsSubMacintoshCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsSubMacintoshCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetSubSpecificTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectBasicWindowsValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsBasikWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsBasikWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetBasicTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectBasicUnixValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsBasikWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsBasikWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetBasicTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteTagsReturnCorrectBasicMacintoshValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsBasikWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsBasikWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetBasicTags();

            byte[] resultBytes = TxtCleaner.DeleteTags(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteTagsAsyncReturnCorrectValue()
        {
            byte[] txtBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/Cases/DeleteTagsAssWindowsCase.txt");
            byte[] expectedBytes = BytesLoadHelper.GetFileBytesArray("TestData/Txt/ExpectedResults/DeleteTagsAssWindowsCaseResult.txt");
            Dictionary<byte, List<TxtTag>> tags = TagsCollectionGeneretor.GetAssSpecificTags();

            byte[] resultBytes = await TxtCleaner.DeleteTagsAsync(txtBytes, tags);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}
