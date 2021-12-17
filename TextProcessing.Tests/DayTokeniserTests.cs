using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class DayTokeniserTests
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
            new WeekDayTokenProcessor()
                .Tokenise(text)
                .As<DayOfWeek>()
                .Should().Be(dayOfWeek);
        }
    }
}
