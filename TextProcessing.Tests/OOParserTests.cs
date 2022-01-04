using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
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

        static Parser<RangeMarker> rangeMarker =
            Parser.Or(
                Parser.IsToken<HypenSymbol>().Select(r => new RangeMarker()),
                Parser.IsToken<JoiningWord>().Select(r => new RangeMarker())
            );

        static Parser<Range<DayOfWeek>> dayRangeParser =
            Parser.IsToken<DayOfWeek>().Then(from =>
                rangeMarker.Then(_ => 
                    Parser.IsToken<DayOfWeek>()
                        .Select(to => new Range<DayOfWeek> { From = from, To = to })));

        static Parser<Range<LocalTime>> timeRangeParser =
            Parser.IsToken<LocalTime>().Then(from =>
                rangeMarker.Then(_ =>
                    Parser.IsToken<LocalTime>()
                        .Select(to => new Range<LocalTime> { From = from, To = to })));

        static Parser<OpenHours> openHoursParser =
            Parser.IsToken<OpenFlag>().Then(_ =>
                dayRangeParser.Then(dr =>
                    timeRangeParser.Select(tr => new OpenHours { Days = dr, Hours = tr})));

        static Parser<List<LocalTime>> tourTimesParser =
            Parser.IsToken<ToursFlag>().Then(_ =>
                Parser.ListOf(
                    Parser.IsToken<LocalTime>())
                        .Select(times => times));

        static Parser<List<DayTime>> eventTimesParser =
            Parser.IsToken<EventsFlag>().Then(_ =>
                Parser.ListOf(dayTimeParser)
                    .Select(times => times));

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

        [Theory]
        [InlineData("Mon to Fri")]
        [InlineData("Monday - Friday")]
        public void DayRangeTests(string text)
        {
            var tokens = Tokenise(text, true);

            var result = dayRangeParser
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

            var result = timeRangeParser
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

            openHoursParser
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

            tourTimesParser
                .Parse(tokens)
                .Success.Should().BeTrue();
        }

        [Theory]
        [InlineData("Events Tuesday 18:00 Wednesday 15:00 Friday 12:00")]
        public void EventsTests(string text)
        {
            var tokens = Tokenise(text, true);

            var result = eventTimesParser
                .Parse(tokens)
                .Success.Should().BeTrue();
        }

        private static Token[] Tokenise(string text, bool fullMatch = false)
        {
            var processor = new Tokeniser(" ",
                new JoiningWordTokenProcessor(),
                new HypenSymbolTokenProcessor(),
                new FlagTokenProcessor(),
                new WeekDayTokenProcessor(),
                new ClockTimeTokenProcessor(),
                new IntegerTokenProcessor());

            var tokens = processor
                .Tokenise(text)
                .ToArray();

            if (fullMatch && tokens.Any(t => t.Is<string>()))
                throw new ApplicationException("Unmatched tokens.");

            return tokens;
        }
    }
}
