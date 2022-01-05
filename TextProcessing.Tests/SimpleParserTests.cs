using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.OO.Parsers;
using TextProcessing.OO.SimpleParsers;
using TextProcessing.OO.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class SimpleParserTests
    {
        [Theory]
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void SimpleParsingSuccess(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            var result = new SimpleDayTimeParser()
                .Parse(tokens);

            result.Success
                .Should().BeTrue();

            var dayTime = result.Value;

            dayTime.Day
                .Should().Be(weekDay);

            dayTime.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("aaa Monday 08:30am")]
        [InlineData("tue 18:301")]
        [InlineData("thurs 12:30pmX")]
        [InlineData("Wed 00:30pm 123")]
        [InlineData("- Sat 12:30pm -")]
        public void SimpleParsingFailure(string text)
        {
            var tokens = Tokenise(text);

            var result = new SimpleDayTimeParser()
                .Parse(tokens);

            result.Success
                .Should().BeFalse();
        }
        private static Token[] Tokenise(string text, bool fullMatch = false)
        {
            var processor = new Tokeniser(" ",
                new JoiningWordTokenParser(),
                new HypenSymbolTokenParser(),
                new FlagTokenParser(),
                new WeekDayTokenParser(),
                new ClockTimeTokenParser(),
                new IntegerTokenParser());

            var tokens = processor
                .Tokenise(text)
                .ToArray();

            if (fullMatch && tokens.Any(t => t.Is<string>()))
                throw new ApplicationException("Unmatched tokens.");

            return tokens;
        }
    }
}
