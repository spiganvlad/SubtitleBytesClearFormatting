using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesLibraryTests
{
    public class SbvCleanerTests
    {
        [Fact]
        public void DeleteFormattingNullParameter()
        {
            byte[] subtitleBytes = null;
            SbvCleaner sbvCleaner = new();

            void act() => sbvCleaner.DeleteFormatting(subtitleBytes);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Sbv subtitle bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteFormattingEmptyParameter()
        {
            byte[] subtitleBytes = Array.Empty<byte>();
            SbvCleaner sbvCleaner = new();

            List<byte> resultBytes = sbvCleaner.DeleteFormatting(subtitleBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectWindowsValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sbv/Cases/SbvWindowsCase.sbv");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sbv/ExpectedResults/SbvWindowsCaseResult.txt");
            SbvCleaner sbvCleaner = new();

            List<byte> resultBytes = sbvCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectUnixValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sbv/Cases/SbvUnixCase.sbv");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sbv/ExpectedResults/SbvUnixCaseResult.txt");
            SbvCleaner sbvCleaner = new();

            List<byte> resultBytes = sbvCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectMacintoshValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sbv/Cases/SbvMacintoshCase.sbv");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sbv/ExpectedResults/SbvMacintoshCaseResult.txt");
            SbvCleaner sbvCleaner = new();

            List<byte> resultBytes = sbvCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteFormattingAsyncReturnCorrectValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Sbv/Cases/SbvWindowsCase.sbv");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Sbv/ExpectedResults/SbvWindowsCaseResult.txt");
            SbvCleaner sbvCleaner = new();

            List<byte> resultBytes = await sbvCleaner.DeleteFormattingAsync(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}