using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessing.Model;
using TextProcessing.OO.Parsers;
using TextProcessing.OO.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class OOParserTests
    {
        // Separate two part element context
        //"Pickup Mon 08:00 dropoff wed 17:00"

        // Range elements
        //"Open Mon to Fri 08:00 - 18:00"

        // Repeating tokens
        //"Tours 10:00 12:00 14:00 17:00 20:00"

        // Repeating complex elements
        //"Events Tuesday 18:00 Wednesday 15:00 Friday 12:00"

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
        public void ComplexParsing2(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            var dayTime = ExpressionParsers.ExplicitDayTimeParser
                .Parse(tokens).Value;

            dayTime.Day
                .Should().Be(weekDay);

            dayTime.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("abb Monday 08:30am")]
        [InlineData("123 tue 18:30")]
        public void BadStartToken(string text)
        {
            var tokens = Tokenise(text);

            ExpressionParsers.ExplicitDayTimeParser
                .Parse(tokens)
                .Success
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Monday 08:30am abb")]
        [InlineData("tue 18:30 123")]
        public void BadEndToken(string text)
        {
            var tokens = Tokenise(text);

            ExpressionParsers.ExplicitDayTimeParser
                .Parse(tokens)
                .Success
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("123 456 789")]
        [InlineData("123 456 789 101 112")]
        public void ListTest(string text)
        {
            var tokens = Tokenise(text);

            var result = Parsers.ListOf(new IsToken<int>())
                .Parse(tokens);

            result.Value
                .Should().HaveCountGreaterThan(2);
        }

        [Theory]
        [InlineData("tue 18:30 tue 18:30 tue 18:30")]
        public void ListDayTimes(string text)
        {
            var tokens = Tokenise(text);

            var result = Parsers.ListOf(ExpressionParsers.DayTimeParser)
                .Parse(tokens);

            result.Value
                .Should().HaveCount(3);
        }

        [Theory]
        [InlineData("Pickup Mon 08:00 dropoff wed 17:00")]
        public void PickupDropOffTests(string text)
        {
            var tokens = Tokenise(text);

            var pickupFlag = Parsers
                .IsToken<PickupFlag>()
                .Then(_ => ExpressionParsers.DayTimeParser);

            var dropOffFlag = Parsers
                .IsToken<DropoffFlag>()
                .Then(_ => ExpressionParsers.DayTimeParser);

            var hybrid  = pickupFlag
                .Then(pu => dropOffFlag
                .Select(dr => new PickupDropoff { Pickup = pu, DropOff = dr }))
                .End();

            var pickupResult = hybrid
                .Parse(tokens);

            pickupResult.Success.Should().BeTrue();
        }

        [Theory]
        [InlineData("Mon to Fri")]
        [InlineData("Monday - Friday")]
        public void DayRangeTests(string text)
        {
            var tokens = Tokenise(text, true);

            var result = ExpressionParsers
                .DayRangeParser
                .Parse(tokens)
                .Value;

            result.From
                .Should().Be(DayOfWeek.Monday);

            result.To
                .Should().Be(DayOfWeek.Friday);
        }

        [Theory]
        [InlineData("8:00 to 23:00")]
        [InlineData("08:00am - 11:00pm")]
        public void TimeRangeTests(string text)
        {
            var tokens = Tokenise(text, true);

            var result = ExpressionParsers
                .TimeRangeParser
                .Parse(tokens)
                .Value;

            result.From
                .Should().Be(new LocalTime(08, 00));

            result.To
                .Should().Be(new LocalTime(23, 00));
        }

        [Theory]
        [InlineData("Open Mon to Fri 08:00 - 18:00")]
        public void OpenHoursTest(string text)
        {
            var tokens = Tokenise(text, true);

            ExpressionParsers
                .OpenHoursParser
                .Parse(tokens)
                .Success.Should().BeTrue();
        }

        [Theory]
        [InlineData("Tours 10:00 12:00 14:00 17:00")]
        [InlineData("Tours 10:00 12:00 14:00 17:00 20:00")]
        [InlineData("Tours 10:00 12:00 14:00 17:00 20:00 22:00")]
        public void TourTimesTests(string text)
        {
            var tokens = Tokenise(text, true);

            ExpressionParsers
                .TourTimesParser
                .Parse(tokens)
                .Success.Should().BeTrue();
        }

        [Theory]
        [InlineData("Events Tuesday 18:00 Wednesday 15:00 Friday 12:00")]
        public void EventsTests(string text)
        {
            var tokens = Tokenise(text, true);

            var result = ExpressionParsers
                .EventTimesParser
                .Parse(tokens)
                .Success.Should().BeTrue();
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
