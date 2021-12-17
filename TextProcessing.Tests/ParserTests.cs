using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Parsers;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void SimpleParsing(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            var dayTime = new SimpleDayTimeParser()
                .Parse(tokens);

            dayTime.Day
                .Should().Be(weekDay);

            dayTime.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void ComplexParsing(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            var dayTime = new DayTimeParser()
                .Parse(tokens)
                .Value;

            dayTime.Day
                .Should().Be(weekDay);

            dayTime.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("abb Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("123 tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        public void BadStartToken(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            new DayTimeParser()
                .Parse(tokens)
                .Success
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Monday 08:30am abb", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30 123", DayOfWeek.Tuesday, 18, 30)]
        public void BadEndToken(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            new DayTimeParser()
                .Parse(tokens)
                .Success
                .Should().BeFalse();
        }


        private static Token[] Tokenise(string text)
        {
            var processor = new Tokeniser(" ",
                new WeekDayTokenProcessor(),
                new ClockTimeTokenProcessor());

            var tokens = processor
                .Tokenise(text)
                .ToArray();

            return tokens;
        }
    }
}
