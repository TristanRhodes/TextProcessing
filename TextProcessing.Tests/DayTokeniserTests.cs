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
        private WeekDayTokeniser _tokeniser = new WeekDayTokeniser();

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
            _tokeniser.IsMatch(text)
                .Should().BeTrue();

            _tokeniser
                .Tokenise(text)
                .Should().BeOfType<Token<DayOfWeek>>()
                .Subject.Value
                .Should().Be(dayOfWeek);
        }
    }
}
