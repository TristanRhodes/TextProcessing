using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;
using TextProcessing.Parsers;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class ParserTests
    {
        static Parser<DayTime> dayTimeParser = new Select<DayTime>(
                (dt, p) => new Is<DayOfWeek>().ParseInto(p, dow => dt.Day = dow),
                (dt, p) => new Is<LocalTime>().ParseInto(p, lt => dt.LocalTime = lt));

        static Parser<DayTime> dayTimeParser2 = 
            new Then<DayOfWeek, DayTime>(
                new Is<DayOfWeek>(),
                dow => new Select<LocalTime, DayTime>(
                    new Is<LocalTime>(),
                    lt => new DayTime { Day = dow, LocalTime = lt }));

        static Parser<DayTime> explicitDayTimeParser = 
            new Beginning<DayTime>(
                new End<DayTime>(dayTimeParser));

        static Parser<DayTime> explicitDayTimeParser2 =
            new Beginning<DayTime>(
                new End<DayTime>(dayTimeParser2));

        [Fact]
        public void NullTest()
        {
            //var joinQuery1 =
            //    DayOfWeek _dow;
            //    LocalTime _lt;
            //    var match = null;
            //    var result = new Is<DayOfWeek>(dow => { _dow = dow })
            //      .Then(new Is<LocalTime>(lt => { _lt = lt})) 
            //    })
            //    if (result != null)
            //      return Fail<T>();
            // 
            // return Success<T>(match);
        }


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
        [InlineData("Monday 08:30am", DayOfWeek.Monday, 8, 30)]
        [InlineData("tue 18:30", DayOfWeek.Tuesday, 18, 30)]
        [InlineData("thurs 12:30pm", DayOfWeek.Thursday, 12, 30)]
        [InlineData("Wed 00:30pm", DayOfWeek.Wednesday, 12, 30)]
        [InlineData("Sat 12:30pm", DayOfWeek.Saturday, 12, 30)]
        public void ComplexParsing3(string text, DayOfWeek weekDay, int hour, int min)
        {
            var tokens = Tokenise(text);

            var dayTime = explicitDayTimeParser2.Parse(tokens)
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

            explicitDayTimeParser
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

            var pickup = new Select<DayTime>(
                (s, p) => new Is<string>(s => s == "Pickup").Parse(p),
                (s, p) => dayTimeParser.ParseInto(p, dt =>
                {
                    s.Day = dt.Day;
                    s.LocalTime = dt.LocalTime;
                }));

            var dropOff = new Select<DayTime>(
                (s, p) => new Is<string>(s => s == "dropoff").Parse(p),
                (s, p) => dayTimeParser.ParseInto(p, dt =>
                {
                    s.Day = dt.Day;
                    s.LocalTime = dt.LocalTime;
                }));

            var hybrid = new Beginning<PickupDropoff>(
                new End<PickupDropoff>(
                    new Select<PickupDropoff>(
                        (s, p) => pickup.ParseInto(p, dt => s.Pickup = dt),
                        (s, p) => dropOff.ParseInto(p, dt => s.DropOff = dt))
            ));

            var pickupResult = hybrid
                .Parse(tokens);

            pickupResult.Success.Should().BeTrue();
        }

        private static Token[] Tokenise(string text)
        {
            var processor = new Tokeniser(" ",
                new WeekDayTokenProcessor(),
                new ClockTimeTokenProcessor(),
                new IntegerTokenProcessor());

            return processor
                .Tokenise(text)
                .ToArray();
        }
    }

    public class PickupDropoff
    {
        public DayTime Pickup { get; set; }

        public DayTime DropOff { get; set; }
    }
}
