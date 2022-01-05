using FluentAssertions;
using NodaTime;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.SpracheParsers;
using Xunit;

namespace TextProcessing.Tests.SpracheParsers
{
    public class SpracheTests
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
        [InlineData("sun", DayOfWeek.Sunday)]
        [InlineData("Monday", DayOfWeek.Monday)]
        [InlineData("mon", DayOfWeek.Monday)]
        [InlineData("Thurs", DayOfWeek.Thursday)]
        [InlineData("thursday", DayOfWeek.Thursday)]
        [InlineData("Fri", DayOfWeek.Friday)]
        [InlineData("Sat", DayOfWeek.Saturday)]
        public void DayMatch(string text, DayOfWeek dayOfWeek)
        {

            TokenParsers
                .DayOfWeek.Parse(text)
                .Should().Be(dayOfWeek);
        }

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

            TokenParsers
                .LocalTime.Parse(text)
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("81:30am")]
        [InlineData("08:62am")]
        [InlineData("20:30am ")]
        [InlineData("10-30am")]
        [InlineData("1:3am")]
        //[InlineData("20:30 ")] // NOTE: Broken due to regex end of word vs end of string.
        [InlineData(" 20:30")]
        [InlineData("10-30")]
        [InlineData("1:3")]
        [InlineData("20:30ampm")]
        public void BadFormatConversion(string text)
        {
            TokenParsers
                .LocalTime.TryParse(text)
                .WasSuccessful
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("13:00am")]
        [InlineData("13:01am")]
        [InlineData("13:00pm")]
        [InlineData("13:01pm")]
        [InlineData("24:00")]
        [InlineData("24:01")]
        [InlineData("25:00")]
        [InlineData("25:01")]
        public void BadConversion(string text)
        {
            TokenParsers
                .LocalTime.TryParse(text)
                .WasSuccessful
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Mon 08:00", DayOfWeek.Monday, 08, 00)]
        public void DayTimeTests(string text, DayOfWeek dow, int hours, int min)
        {
            var result = ExpressionParsers.DayTime
                .Parse(text);
            result.Day.Should().Be(dow);
            result.LocalTime.Should().Be(new LocalTime(hours, min));


            result = ExpressionParsers.DayTimeDelegate
                .Parse(text);
            result.Day.Should().Be(dow);
            result.LocalTime.Should().Be(new LocalTime(hours, min));
        }

        [Theory]
        [InlineData("Mon08:00")]
        public void FailDayTimeTests(string text)
        {
            ExpressionParsers.DayTime
                .TryParse(text).WasSuccessful
                .Should().BeFalse();

            ExpressionParsers.DayTimeDelegate
                .TryParse(text).WasSuccessful
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("pickup")]
        [InlineData("Pickup")]
        public void Pickup(string text)
        {
            TokenParsers.PickupFlag
                .TryParse(text).WasSuccessful
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("Pickup Mon 08:00")]
        public void PickupDayTimeTests(string text)
        {
            var parser = from pickupFlag in TokenParsers.PickupFlag
                         from _ in Parse.WhiteSpace
                         from pickup in ExpressionParsers.DayTime
                         select pickup;

            parser
                .End()
                .Parse(text);
        }

        [Theory]
        [InlineData("Pickup Mon 08:00 dropoff wed 17:00")]
        public void PickupDropOffTests(string text)
        {
            ExpressionParsers
                .PickupDropOff
                .End()
                .Parse(text);
        }

        [Theory]
        [InlineData("Mon to Fri")]
        [InlineData("Monday - Friday")]
        [InlineData("monday-fri")]
        public void DayRangeTests(string text)
        {
            var result = ExpressionParsers
                .DayRange
                .End()
                .Parse(text);

            result.From
                .Should().Be(DayOfWeek.Monday);
            
            result.To
                .Should().Be(DayOfWeek.Friday);
        }

        [Theory]
        [InlineData("8:00 to 23:00")]
        [InlineData("08:00am - 11:00pm")]
        [InlineData("08:00am-11:00pm")]
        public void TimeRangeTests(string text)
        {
            var result = ExpressionParsers
                .TimeRange
                .End()
                .Parse(text);

            result.From
                .Should().Be(new LocalTime(08, 00));

            result.To
                .Should().Be(new LocalTime(23, 00));
        }

        [Theory]
        [InlineData("Open Mon to Fri 08:00 - 18:00")]
        public void OpenHoursTest(string text)
        {
            ExpressionParsers
                .OpenHours
                .End()
                .Parse(text);
        }

        [Theory]
        [InlineData("Tours 10:00 12:00 14:00 17:00")]
        [InlineData("Tours 10:00 12:00 14:00 17:00 20:00")]
        [InlineData("Tours 10:00 12:00 14:00 17:00 20:00 22:00")]
        public void TourTimesTests(string text)
        {
            ExpressionParsers
                .TourTimes
                .End()
                .Parse(text);
        }

        [Theory]
        [InlineData("Events Tuesday 18:00 Wednesday 15:00 Friday 12:00")]
        public void EventsTests(string text)
        {
            ExpressionParsers
                .EventSchedule
                .End()
                .Parse(text);
        }
    }
}
