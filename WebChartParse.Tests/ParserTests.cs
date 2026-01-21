using System;
using WebChartParse.Models;
using Xunit;

namespace WebChartParse.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("1+2", 3)]
        [InlineData("2*3+4", 10)]
        [InlineData("2+3*4", 14)]
        [InlineData("2^(3)", 8)]
        [InlineData("2*(3+4)", 14)]
        [InlineData("sqrt(4)", 2)]
        [InlineData("sin(0)", 0)]
        [InlineData("1.5+1", 2.5)]
        [InlineData("-2^2", -4)]
        [InlineData("(2+1)(3+1)", 12)]
        [InlineData("2pi", 2 * Math.PI)]
        [InlineData("max(2,5)", 5)]
        [InlineData("pow(2,3)", 8)]
        public void Parse_ReturnsExpectedValue(string expression, double expected)
        {
            Parser parser = new Parser();

            double result = parser.Parse(expression);

            Assert.Equal(expected, result, 6);
        }

        [Theory]
        [InlineData("√(9)", 3)]
        [InlineData("π + 1", Math.PI + 1)]
        [InlineData("2×3", 6)]
        [InlineData("8÷2", 4)]
        public void Parse_SupportsUnicodeSymbols(string expression, double expected)
        {
            Parser parser = new Parser();

            double result = parser.Parse(expression);

            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void Parse_DivideByZero_ReturnsZero()
        {
            Parser parser = new Parser();

            double result = parser.Parse("1/0");

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyOrWhitespace_ReturnsZero(string expression)
        {
            Parser parser = new Parser();

            double result = parser.Parse(expression);

            Assert.Equal(0, result);
        }
    }
}
