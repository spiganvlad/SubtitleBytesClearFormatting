using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesClearFormattingTest
{
    public class SrtCleanerTests
    {
        [Fact]
        public void DeleteFormattingNullParameter()
        {
            byte[] subtitleBytes = null;
            SrtCleaner srtCleaner = new();

            void act() { srtCleaner.DeleteFormatting(subtitleBytes); };

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Srt subtitle bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteFormattingEmptyParameter()
        {
            byte[] subtitleBytes = Array.Empty<byte>();
            SrtCleaner srtCleaner = new();

            List<byte> resultBytes = srtCleaner.DeleteFormatting(subtitleBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectWindowsValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Srt/Cases/SrtWindowsCase.srt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Srt/ExpectedResults/SrtWindowsCaseResult.txt");
            SrtCleaner srtCleaner = new();

            List<byte> resultBytes = srtCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectUnixValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Srt/Cases/SrtUnixCase.srt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Srt/ExpectedResults/SrtUnixCaseResult.txt");
            SrtCleaner srtCleaner = new();

            List<byte> resultBytes = srtCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectMacintoshValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Srt/Cases/SrtMacintoshCase.srt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Srt/ExpectedResults/SrtMacintoshCaseResult.txt");
            SrtCleaner srtCleaner = new();

            List<byte> resultBytes = srtCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteFormattingAsyncReturnCorrectValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Srt/Cases/SrtWindowsCase.srt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Srt/ExpectedResults/SrtWindowsCaseResult.txt");
            SrtCleaner srtCleaner = new();

            List<byte> resultBytes = await srtCleaner.DeleteFormattingAsync(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}
