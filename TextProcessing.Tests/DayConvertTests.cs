using FluentAssertions;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace TextProcessing.Tests
{
    public class DayConvertTests
    {
        [Theory]
        [InlineData("mon")]
        [InlineData("Mon")]
        [InlineData("monday")]
        [InlineData("Monday")]
        public void Monday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Monday);
        }

        [Theory]
        [InlineData("tue")]
        [InlineData("Tue")]
        [InlineData("tuesday")]
        [InlineData("Tuesday")]
        public void Tuesday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Tuesday);
        }

        [Theory]
        [InlineData("wed")]
        [InlineData("Wed")]
        [InlineData("wednesday")]
        [InlineData("Wednesday")]
        public void Wednesday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Wednesday);
        }

        [Theory]
        [InlineData("thu")]
        [InlineData("Thu")]
        [InlineData("thurs")]
        [InlineData("Thurs")]
        [InlineData("thursday")]
        [InlineData("Thursday")]
        public void Thursday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Thursday);
        }

        [Theory]
        [InlineData("fri")]
        [InlineData("Fri")]
        [InlineData("friday")]
        [InlineData("Friday")]
        public void Friday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Friday);
        }

        [Theory]
        [InlineData("sat")]
        [InlineData("Sat")]
        [InlineData("saturday")]
        [InlineData("Saturday")]
        public void Saturday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Saturday);
        }

        [Theory]
        [InlineData("sun")]
        [InlineData("Sun")]
        [InlineData("sunday")]
        [InlineData("Sunday")]
        public void Sunday(string text)
        {
            Convert(text)
                .Should().Be(DayOfWeek.Sunday);
        }

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
            Convert(text)
                .Should().Be(dayOfWeek);
        }

        [Theory]
        [InlineData("nop3")]
        [InlineData("invalid")]
        [InlineData("123")]
        [InlineData("]#!$%")]
        public void Invalid(string text)
        {
            Convert(text)
                .Should().Be(null);
        }

        DayOfWeek? Convert(string token)
        {
            var match = Regex.Match(token, "^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$");
            if (!match.Success)
                return null;

            if (match.Groups["Monday"].Success)
                return DayOfWeek.Monday;

            if (match.Groups["Tuesday"].Success)
                return DayOfWeek.Tuesday;

            if (match.Groups["Wednesday"].Success)
                return DayOfWeek.Wednesday;

            if (match.Groups["Thursday"].Success)
                return DayOfWeek.Thursday;

            if (match.Groups["Friday"].Success)
                return DayOfWeek.Friday;

            if (match.Groups["Saturday"].Success)
                return DayOfWeek.Saturday;

            if (match.Groups["Sunday"].Success)
                return DayOfWeek.Sunday;

            throw new NotSupportedException($"Bad Format: {token}");
        }
    }
}
