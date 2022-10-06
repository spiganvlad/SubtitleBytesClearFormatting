using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesClearFormattingTest
{
    public class AssCleanerTests
    {
        [Fact]
        public void DeleteFormattingNullParameter()
        {
            byte[] subtitleBytes = null;
            AssCleaner assCleaner = new();

            void act() => assCleaner.DeleteFormatting(subtitleBytes);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Ass subtitle bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteFormattingEmptyParameter()
        {
            byte[] subtitleBytes = Array.Empty<byte>();
            AssCleaner assCleaner = new();

            List<byte> resultBytes = assCleaner.DeleteFormatting(subtitleBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectWindowsValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Ass/Cases/AssWindowsCase.ass");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Ass/ExpectedResults/AssWindowsCaseResult.txt");
            AssCleaner assCleaner = new();

            List<byte> resultBytes = assCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectUnixValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Ass/Cases/AssUnixCase.ass");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Ass/ExpectedResults/AssUnixCaseResult.txt");
            AssCleaner assCleaner = new();

            List<byte> resultBytes = assCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectMacintoshValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Ass/Cases/AssMacintoshCase.ass");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Ass/ExpectedResults/AssMacintoshCaseResult.txt");
            AssCleaner assCleaner = new();

            List<byte> resultBytes = assCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteFormattingAsyncReturnCorrectValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Ass/Cases/AssWindowsCase.ass");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Ass/ExpectedResults/AssWindowsCaseResult.txt");
            AssCleaner assCleaner = new();

            List<byte> resultBytes = await assCleaner.DeleteFormattingAsync(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}