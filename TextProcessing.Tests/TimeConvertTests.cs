using FluentAssertions;
using NodaTime;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace TextProcessing.Tests
{
    public class TimeConvertTests
    {
        [Theory]
        [InlineData("8:30am", 8, 30)]
        [InlineData("08:30am", 8, 30)]
        [InlineData("8:30pm", 20, 30)]
        [InlineData("08:30pm", 20, 30)]
        [InlineData("8:30", 8, 30)]
        [InlineData("08:30", 8, 30)]
        [InlineData("20:30", 20, 30)]
        public void TimeConvertTest(string text, int hour, int min)
        {
            MatchTime(text)
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("20:30am ")]
        [InlineData("10-30am")]
        [InlineData("1:3am")]
        [InlineData("20:30 ")]
        [InlineData("10-30")]
        [InlineData("1:3")]
        [InlineData("20:30ampm")]
        public void InvalidTimeConvertTest(string text)
        {
            MatchTime(text)
                .Should().BeNull();
        }

        public LocalTime? MatchTime(string token)
        {
            var match = Regex.Match(token, @"^(?<hr>\d{1,2}):(?<min>\d{2})((?<am>am)|(?<pm>pm))?$");
            if (!match.Success)
                return null;

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"];
            var pm = match.Groups["pm"];

            if (!am.Success && !pm.Success)
            {
                return new LocalTime(hour, min);
            }
            else if (am.Success)
            {
                return new LocalTime(hour, min);
            }
            else if (pm.Success)
            {
                return new LocalTime(hour + 12, min);
            }

            throw new NotSupportedException($"Bad Format: {token}");
        }
    }
}
