using FluentAssertions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Functional.Parsers;
using TextProcessing.Functional.Tokenisers;
using Xunit;

namespace TextProcessing.Tests.Functional
{
    public class LanguageAwareTokeniserTests
    {
        LanguageAwareTokeniser _tokeniser;

        public LanguageAwareTokeniserTests()
        {
            _tokeniser = new LanguageAwareTokeniser(" ");
            _tokeniser.AddLanguage(Language.English,
                EnglishTokenParsers.JoiningWord,
                EnglishTokenParsers.Flags,
                EnglishTokenParsers.WeekDay,
                CommonTokenParsers.HypenSymbol,
                CommonTokenParsers.ClockTime,
                CommonTokenParsers.Integer);

            _tokeniser.AddLanguage(Language.German,
                GermanTokenParsers.JoiningWord,
                GermanTokenParsers.Flags,
                GermanTokenParsers.WeekDay,
                CommonTokenParsers.HypenSymbol,
                CommonTokenParsers.ClockTime,
                CommonTokenParsers.Integer);
        }

        [Theory]
        [InlineData("Mon to Fri", Language.English)]
        [InlineData("Monday - Friday", Language.English)]
        [InlineData("Mon bis frei", Language.German)]
        [InlineData("Montag - Freitag", Language.German)]
        public void MultiLanguageDayRange(string text, Language language)
        {
            var tokens = _tokeniser
                .Tokenise(text, language)
                .ToArray();

            var result = ExpressionParsers
                .DayRangeParser
                .Parse(tokens)
                .Value;

            result.From
                .Should().Be(DayOfWeek.Monday);

            result.To
                .Should().Be(DayOfWeek.Friday);
        }


        [Theory]
        [InlineData("Open Mon to Fri 08:00 - 18:00", Language.English)]
        [InlineData("Offen mon bis frei 08:00 - 18:00", Language.German)]
        [InlineData("open monday - friday 08:00am to 06:00pm", Language.English)]
        [InlineData("offen montag - freitag 08:00am bis 06:00pm", Language.German)]
        public void MultiLanguageOpenHours(string text, Language language)
        {
            var tokens = _tokeniser
                .Tokenise(text, language)
                .ToArray();

            var result = ExpressionParsers
                .OpenHoursParser
                .Parse(tokens)
                .Value;

            result.Days.From
                .Should().Be(DayOfWeek.Monday);
            result.Days.To
                .Should().Be(DayOfWeek.Friday);

            result.Hours.From
                .Should().Be(new LocalTime(08,00));
            result.Hours.To
                .Should().Be(new LocalTime(18, 00));
        }

    }
}
