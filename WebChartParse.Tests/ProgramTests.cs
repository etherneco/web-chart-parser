using System;
using ConsoleApp4;
using Xunit;

namespace WebChartParse.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void GetWordFromText_ReturnsRequestedWord()
        {
            string result = Program.GetWordFromText("one two three", 2);

            Assert.Equal("two", result);
        }

        [Fact]
        public void GetWordFromText_ThrowsOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => Program.GetWordFromText(null, 1));
        }

        [Fact]
        public void GetWordFromText_ThrowsOnOutOfRangeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.GetWordFromText("one", 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.GetWordFromText("one", 2));
        }

        [Fact]
        public void Reverse_ReturnsReversedString()
        {
            string result = Program.Reverse("abcd");

            Assert.Equal("dcba", result);
        }

        [Fact]
        public void Reverse_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => Program.Reverse(null));
        }
    }
}
