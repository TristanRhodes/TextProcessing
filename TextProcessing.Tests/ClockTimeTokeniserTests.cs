using FluentAssertions;
using NodaTime;
using System;
using TextProcessing.OOTokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class ClockTimeTokeniserTests
    {
        [Theory]
        [InlineData("8:30am", 8, 30)]
        [InlineData("08:30am", 8, 30)]
        [InlineData("8:30pm", 20, 30)]
        [InlineData("08:30pm", 20, 30)]
        [InlineData("8:30", 8, 30)]
        [InlineData("08:30", 8, 30)]
        [InlineData("20:30", 20, 30)]
        [InlineData("18:30", 18, 30)]
        public void TimeConvertTest(string text, int hour, int min)
        {
            new ClockTimeTokenProcessor()
                .Tokenise(text)
                .As<LocalTime>()
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("81:30am")]
        [InlineData("08:62am")]
        [InlineData("20:30am ")]
        [InlineData("10-30am")]
        [InlineData("1:3am")]
        [InlineData("20:30 ")]
        [InlineData("10-30")]
        [InlineData("1:3")]
        [InlineData("20:30ampm")]
        public void BadFormatConversion(string text)
        {
            new ClockTimeTokenProcessor()
                .IsMatch(text)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("13:00am")]
        [InlineData("13:01am")]
        [InlineData("13:00pm")]
        [InlineData("13:01pm")]
        public void Bad12HourConversion(string text)
        {
            new ClockTimeTokenProcessor()
                .IsMatch(text)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("24:00")]
        [InlineData("24:01")]
        [InlineData("25:00")]
        [InlineData("25:01")]
        public void Bad24HourConversion(string text)
        {
            new ClockTimeTokenProcessor()
                .IsMatch(text)
                .Should().BeFalse();
        }
    }
}
