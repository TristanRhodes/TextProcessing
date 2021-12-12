using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class TokenProcessingTests
    {
        private TokenProcessor _tokenProcessor;

        public TokenProcessingTests()
        {
            _tokenProcessor = new TokenProcessor(
                new DayTokeniser(),
                new ClockTimeTokeniser());
        }

        [Theory]
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void Test(string text, DayOfWeek weekDay, int hour, int min)
        {
            var parts = text.Split(" ");

            var tokens = _tokenProcessor
                .Tokenise(parts);

            tokens[0]
                .Should().BeOfType<DayToken>()
                .Subject.DayOfWeek
                .Should().Be(weekDay);

            tokens[1]
                .Should().BeOfType<TimeToken>()
                .Subject.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        // 12 hr
        [InlineData("00:00am", 00, 00)]
        [InlineData("00:01am", 00, 01)]
        [InlineData("12:00am", 00, 00)]
        [InlineData("12:01am", 00, 01)]
        [InlineData("00:00pm", 12, 00)]
        [InlineData("00:01pm", 12, 01)]
        [InlineData("12:00pm", 12, 00)]
        [InlineData("12:01pm", 12, 01)]
        // 24 hr
        [InlineData("00:00", 00, 00)]
        [InlineData("00:01", 00, 01)]
        [InlineData("12:00", 12, 00)]
        [InlineData("12:01", 12, 01)]
        [InlineData("23:00", 23, 00)]
        [InlineData("23:01", 23, 01)]
        public void TimeTest(string text, int hour, int min)
        {
            var parts = text.Split(" ");

            var tokens = _tokenProcessor
                .Tokenise(parts);

            tokens[0]
                .Should().BeOfType<TimeToken>()
                .Subject.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        // 12 hr
        [InlineData("13:00am")]
        [InlineData("13:01am")]
        [InlineData("13:00pm")]
        [InlineData("13:01pm")]
        // 24 hr
        [InlineData("24:00")]
        [InlineData("24:01")]
        [InlineData("25:00")]
        [InlineData("25:01")]
        public void InvalidTimeTest(string text)
        {
            var parts = text.Split(" ");

            var tokens = _tokenProcessor
                .Tokenise(parts);

            tokens[0]
                .Should().BeOfType<UnknownToken>();
        }
    }
}
