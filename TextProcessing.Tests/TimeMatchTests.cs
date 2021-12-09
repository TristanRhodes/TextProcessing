using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace TextProcessing.Tests
{
    public class TimeMatchTests
    {
        [Theory]
        // 12 Hr
        [InlineData("8:30am")]
        [InlineData("08:30am")]
        [InlineData("8:30pm")]
        [InlineData("08:30pm")]
        // 24 Hr
        [InlineData("20:30")]
        [InlineData("10:30")]
        [InlineData("1:30")]
        public void TimeTest(string text)
        {
            MatchTime(text)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(" 20:30 ")]
        [InlineData("10-30")]
        [InlineData("1:3")]
        [InlineData("20:30am ")]
        [InlineData("10-30am")]
        [InlineData("1:3am")]
        public void InvalidTimeTest(string text)
        {
            MatchTime(text)
                .Should().BeFalse();
        }

        public bool MatchTime(string text)
        {
            return Regex.IsMatch(text, @"^\d{1,2}:\d{2}(am|pm)?$");
        }
    }
}
