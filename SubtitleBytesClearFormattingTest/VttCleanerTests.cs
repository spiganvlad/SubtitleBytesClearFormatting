using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using SubtitleBytesClearFormatting.Cleaners;
using SubtitleBytesClearFormattingTest.Helpers;

namespace SubtitleBytesClearFormattingTest
{
    public class VttCleanerTests
    {
        [Fact]
        public void DeleteFormattingNullParameter()
        {
            byte[] subtitleBytes = null;
            VttCleaner vttCleaner = new();

            void act() { vttCleaner.DeleteFormatting(subtitleBytes); };

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Contains("Vtt subtitle bytes cannot be null.", exception.Message);
        }

        [Fact]
        public void DeleteFormattingEmptyParameter()
        {
            byte[] subtitleBytes = Array.Empty<byte>();
            VttCleaner vttCleaner = new();

            List<byte> resultBytes = vttCleaner.DeleteFormatting(subtitleBytes);

            Assert.Empty(resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectWindowsValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Vtt/Cases/VttWindowsCase.vtt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Vtt/ExpectedResults/VttWindowsCaseResult.txt");
            VttCleaner vttCleaner = new();

            List<byte> resultBytes = vttCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectUnixValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Vtt/Cases/VttUnixCase.vtt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Vtt/ExpectedResults/VttUnixCaseResult.txt");
            VttCleaner vttCleaner = new();

            List<byte> resultBytes = vttCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public void DeleteFormattingReturnCorrectMacintoshValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Vtt/Cases/VttMacintoshCase.vtt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Vtt/ExpectedResults/VttMacintoshCaseResult.txt");
            VttCleaner vttCleaner = new();

            List<byte> resultBytes = vttCleaner.DeleteFormatting(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }

        [Fact]
        public async Task DeleteFormattingAsyncReturnCorrectValue()
        {
            byte[] subtitleBytes = BytesLoadHelper.GetFileBytesArray("./TestData/Vtt/Cases/VttWindowsCase.vtt");
            List<byte> expectedBytes = BytesLoadHelper.GetFileBytesList("./TestData/Vtt/ExpectedResults/VttWindowsCaseResult.txt");
            VttCleaner vttCleaner = new();

            List<byte> resultBytes = await vttCleaner.DeleteFormattingAsync(subtitleBytes);

            Assert.Equal(expectedBytes, resultBytes);
        }
    }
}
