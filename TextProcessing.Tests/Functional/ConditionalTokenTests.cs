using FluentAssertions;
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
    public class ConditionalTokenTests
    {
        [Theory]
        [InlineData("1", false)]
        [InlineData("10", false)]
        [InlineData("101", true)]
        [InlineData("1000", true)]
        public void DelegateParser(string text, bool succeed)
        {
            var tokens = Tokenise(text);
            Parsers
                .IsToken<int>(i => i > 100)
                .Parse(tokens)
                .Success
                .Should().Be(succeed);
        }

        public static Token[] Tokenise(string text, bool fullMatch = false)
        {
            var processor = new Tokeniser(" ",
                TokenParsers.Integer);

            var tokens = processor
                .Tokenise(text)
                .ToArray();

            if (fullMatch && tokens.Any(t => t.Is<string>()))
                throw new ApplicationException("Unmatched tokens.");

            return tokens;
        }
    }
}
