using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.OO.Tokenisers;
using Xunit;

namespace TextProcessing.Tests.OO
{
    public class MultiPartTokeniserTests
    {
        [Theory]
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void TokenProcessing(string text, DayOfWeek weekDay, int hour, int min)
        {
            var processor = new Tokeniser(" ",
                new WeekDayTokenParser(),
                new ClockTimeTokenParser());

            var results = processor
                .Tokenise(text)
                .ToArray();

            results[0].As<DayOfWeek>()
                .Should().Be(weekDay);

            results[1].As<LocalTime>()
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("aaa Monday 08:30am 123", DayOfWeek.Monday, 8, 30)]
        [InlineData("bbb tue 18:30 456", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("ccc thurs 12:30pm 789", DayOfWeek.Thursday, 12, 30)]
        [InlineData("ddd Wed 00:30pm 101", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("eeee Sat 12:30pm 112", DayOfWeek.Saturday, 12, 30)]
        public void TokenProcessingWithNoise(string text, DayOfWeek weekDay, int hour, int min)
        {
            var processor = new Tokeniser(" ",
                new WeekDayTokenParser(),
                new ClockTimeTokenParser(),
                new IntegerTokenParser());

            var results = processor
                .Tokenise(text)
                .ToArray();

            results[0].Is<string>()
                .Should().BeTrue();

            results[1].As<DayOfWeek>()
                .Should().Be(weekDay);

            results[2].As<LocalTime>()
                .Should().Be(new LocalTime(hour, min));

            results[3].Is<int>()
                .Should().BeTrue();
        }
    }
}
