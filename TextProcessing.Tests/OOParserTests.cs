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
                new Is<DayOfWeek>(),
                dow => new Select<LocalTime, DayTime>(
                    new Is<LocalTime>(),
                    lt => new DayTime { Day = dow, LocalTime = lt }));

        //public static Parser<OneLineTariff> LineParserDelegates =
        //    MinBoundPrefix.Optional().Then(
        //        minBound => PricedTimePeriodParsers.MultiPriceTimePeriodParser.Then(
        //            timePeriod => RepeaterSuffix.Optional().Select(
        //                repeater => new OneLineTariff(minBound.GetOrDefault(), timePeriod, repeater.GetOrDefault()))));

        //[Fact]
        public void FluentParserBuilder()
        {
            string text = "Monday 08:30am";

            var parserBuilder = new ParserBuilder<DayTime>()
                .Single<DayOfWeek>((bd, dow) =>
                    bd.Single<LocalTime>((bl, lt) => 
                        bl.Select(new DayTime(dow, lt))));

            var parser = parserBuilder().Build();

            var tokens = Tokenise(text);

            var dayTime = parser
                .Parse(tokens)
                .Value;

            dayTime.Day
                .Should().Be(DayOfWeek.Monday);

            dayTime.LocalTime
                .Should().Be(new LocalTime(08, 30));
        }

        static Parser<DayTime> explicitDayTimeParser =
            new Beginning<DayTime>(
                new End<DayTime>(dayTimeParser));

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

            var result = new ListOf<Int32>(new Is<int>())
                .Parse(tokens);

            result.Value
                .Should().HaveCountGreaterThan(2);
        }

        [Theory]
        [InlineData("tue 18:30 tue 18:30 tue 18:30")]
        public void ListDayTimes(string text)
        {
            var tokens = Tokenise(text);

            var result = new ListOf<DayTime>(dayTimeParser)
                .Parse(tokens);

            result.Value
                .Should().HaveCount(3);
        }

        [Theory]
        [InlineData("Pickup Mon 08:00 dropoff wed 17:00")]
        public void PickupDropOffTests(string text)
        {
            var tokens = Tokenise(text);

            var pickupFlag = new Then<PickupFlag, DayTime>(
                new Is<PickupFlag>(),
                pu => new Select<DayTime, DayTime>(
                    dayTimeParser,
                    dt => dt));

            var dropOffFlag = new Then<DropoffFlag, DayTime>(
                new Is<DropoffFlag>(),
                pu => new Select<DayTime, DayTime>(
                    dayTimeParser,
                    dt => dt));

            var hybrid = new Beginning<PickupDropoff>(
                new End<PickupDropoff>(
                    new Then<DayTime, PickupDropoff>(
                        pickupFlag,
                        pu => new Select<DayTime, PickupDropoff>(
                            dropOffFlag,
                            dr => new PickupDropoff { Pickup = pu, DropOff = dr }))));

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
