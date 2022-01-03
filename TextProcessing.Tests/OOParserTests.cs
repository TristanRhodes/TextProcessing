using FluentAssertions;
using NodaTime;
using System;
using System.Linq;
using TextProcessing.Model;
using TextProcessing.OOParsers;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class OOParserTests
    {
        static Parser<DayTime> dayTimeParser =
            new Then<DayOfWeek, DayTime>(
                new IsToken<DayOfWeek>(),
                dow => new Select<LocalTime, DayTime>(
                    new IsToken<LocalTime>(),
                    lt => new DayTime { Day = dow, LocalTime = lt }));

        static Parser<DayTime> dayTimeFluentParser =
            Parser.IsToken<DayOfWeek>().Then(dow => 
                Parser.IsToken<LocalTime>().Select(lt =>
                    new DayTime { Day = dow, LocalTime = lt }));

        static Parser<DayTime> explicitDayTimeParser =
            dayTimeParser.End();

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

            var dayTime = explicitDayTimeParser.Parse(tokens)
                .Value;

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

            explicitDayTimeParser
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

            explicitDayTimeParser
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

            var result = Parser.ListOf(new IsToken<int>())
                .Parse(tokens);

            result.Value
                .Should().HaveCountGreaterThan(2);
        }

        [Theory]
        [InlineData("tue 18:30 tue 18:30 tue 18:30")]
        public void ListDayTimes(string text)
        {
            var tokens = Tokenise(text);

            var result = Parser.ListOf(dayTimeParser)
                .Parse(tokens);

            result.Value
                .Should().HaveCount(3);
        }

        [Theory]
        [InlineData("Pickup Mon 08:00 dropoff wed 17:00")]
        public void PickupDropOffTests(string text)
        {
            var tokens = Tokenise(text);

            var pickupFlag = Parser
                .IsToken<PickupFlag>()
                .Then(_ => dayTimeParser);

            var dropOffFlag = Parser
                .IsToken<DropoffFlag>()
                .Then(_ => dayTimeParser);

            var hybrid  = pickupFlag
                .Then(pu => dropOffFlag
                .Select(dr => new PickupDropoff { Pickup = pu, DropOff = dr }))
                .End();

            var pickupResult = hybrid
                .Parse(tokens);

            pickupResult.Success.Should().BeTrue();
        }

        private static Token[] Tokenise(string text)
        {
            var processor = new Tokeniser(" ",
                new PickupDropOffFlagProcessor(),
                new WeekDayTokenProcessor(),
                new ClockTimeTokenProcessor(),
                new IntegerTokenProcessor());

            return processor
                .Tokenise(text)
                .ToArray();
        }
    }
}
