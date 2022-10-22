using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesClearFormattingTest
{
    public class SubCleanerTests
    {
        [Fact]
        public void DeleteFormattingNullParameter()
        {
            byte[] subtitleBytes = null;
            SubCleaner subCleaner = new();

            void act() { subCleaner.DeleteFormatting(subtitleBytes); };

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Sub subtitle bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteFormattingEmptyParameter()
        {
            byte[] subtitleBytes = Array.Empty<byte>();
            SubCleaner subCleaner = new();

            List<byte> resultBytes = subCleaner.DeleteFormatting(subtitleBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectWindowsValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sub/Cases/SubWindowsCase.sub");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sub/ExpectedResults/SubWindowsCaseResult.txt");
            SubCleaner subCleaner = new();

            List<byte> resultBytes = subCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectUnixValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sub/Cases/SubUnixCase.sub");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sub/ExpectedResults/SubUnixCaseResult.txt");
            SubCleaner subCleaner = new();

            List<byte> resultBytes = subCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectMacintoshValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sub/Cases/SubMacintoshCase.sub");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sub/ExpectedResults/SubMacintoshCaseResult.txt");
            SubCleaner subCleaner = new();

            List<byte> resultBytes = subCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteFormattingAsyncReturnCorrectValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sub/Cases/SubWindowsCase.sub");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sub/ExpectedResults/SubWindowsCaseResult.txt");
            SubCleaner subCleaner = new();

            List<byte> resultBytes = await subCleaner.DeleteFormattingAsync(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}
