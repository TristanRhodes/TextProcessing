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

namespace TextProcessing.Tests
{
    public class SpracheTests
    {
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

            ComponentParsers
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

            ComponentParsers
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
            ComponentParsers
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
            ComponentParsers
                .LocalTime.TryParse(text)
                .WasSuccessful
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Mon 08:00", DayOfWeek.Monday, 08, 00)]
        public void DayTimeTests(string text, DayOfWeek dow, int hours, int min)
        {
            var result = ComponentParsers.DayTime
                .Parse(text);
            result.Day.Should().Be(dow);
            result.LocalTime.Should().Be(new LocalTime(hours, min));


            result = ComponentParsers.DayTimeDelegate
                .Parse(text);
            result.Day.Should().Be(dow);
            result.LocalTime.Should().Be(new LocalTime(hours, min));
        }

        [Theory]
        [InlineData("Mon08:00")]
        public void FailDayTimeTests(string text)
        {
            ComponentParsers.DayTime
                .TryParse(text).WasSuccessful
                .Should().BeFalse();

            ComponentParsers.DayTimeDelegate
                .TryParse(text).WasSuccessful
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("pickup")]
        [InlineData("Pickup")]
        public void Pickup(string text)
        {
            ComponentParsers.PickupFlag
                .TryParse(text).WasSuccessful
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("Pickup Mon 08:00")]
        public void PickupDayTimeTests(string text)
        {
            var parser = from pickupFlag in ComponentParsers.PickupFlag
                         from _1 in Parse.WhiteSpace
                         from pickup in ComponentParsers.DayTime
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
    }
}
