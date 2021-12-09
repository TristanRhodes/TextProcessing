using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace TextProcessing.Tests
{
    public class DayMatchTest
    {
        [Theory]
        [InlineData("mon")]
        [InlineData("Mon")]
        [InlineData("monday")]
        [InlineData("Monday")]
        public void Monday(string text)
        {
            Regex.IsMatch(text, "^([Mm]on(day)?)$")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("tue")]
        [InlineData("Tue")]
        [InlineData("tuesday")]
        [InlineData("Tuesday")]
        public void Tuesday(string text)
        {
            Regex.IsMatch(text, "^([Tt]ue(sday)?)$")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("wed")]
        [InlineData("Wed")]
        [InlineData("wednesday")]
        [InlineData("Wednesday")]
        public void Wednesday(string text)
        {
            Regex.IsMatch(text, "^([Ww]ed(nesday)?)$")
                .Should().BeTrue();
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
            Regex.IsMatch(text, "^([Tt]hu(rs(day)?)?)$")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("fri")]
        [InlineData("Fri")]
        [InlineData("friday")]
        [InlineData("Friday")]
        public void Friday(string text)
        {
            Regex.IsMatch(text, "^([Ff]ri(day)?)$")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("sat")]
        [InlineData("Sat")]
        [InlineData("saturday")]
        [InlineData("Saturday")]
        public void Saturday(string text)
        {
            Regex.IsMatch(text, "^([Ss]at(urday)?)$")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("sun")]
        [InlineData("Sun")]
        [InlineData("sunday")]
        [InlineData("Sunday")]
        public void Sunday(string text)
        {
            Regex.IsMatch(text, "^([Ss]un(day)?)$")
                .Should().BeTrue();
        }
    }
}
