﻿using FluentAssertions;
using NodaTime;
using System;
using TextProcessing.Tokenisers;
using Xunit;

namespace TextProcessing.Tests
{
    public class ClockTimeTokeniserTests
    {
        private TimeTokeniser _tokeniser = new TimeTokeniser();

        [Theory]
        [InlineData("8:30am", 8, 30)]
        [InlineData("08:30am", 8, 30)]
        [InlineData("8:30pm", 20, 30)]
        [InlineData("08:30pm", 20, 30)]
        [InlineData("8:30", 8, 30)]
        [InlineData("08:30", 8, 30)]
        [InlineData("20:30", 20, 30)]
        public void TimeConvertTest(string text, int hour, int min)
        {
            _tokeniser.IsMatch(text)
                .Should().BeTrue();

            _tokeniser
                .Tokenise(text)
                .Should().BeOfType<Time>()
                .Subject.LocalTime
                .Should().Be(new LocalTime(hour, min));
        }

        [Theory]
        [InlineData("20:30am ")]
        [InlineData("10-30am")]
        [InlineData("1:3am")]
        [InlineData("20:30 ")]
        [InlineData("10-30")]
        [InlineData("1:3")]
        [InlineData("20:30ampm")]
        public void InvalidTimeConvertTest(string text)
        {
            _tokeniser.IsMatch(text)
                .Should().BeFalse();
        }
    }
}
