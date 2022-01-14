using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;
using TextProcessing.OO.Tokenisers;
using Xunit;

namespace TextProcessing.Tests.OO
{
    public class RegexParserTests
    {
        [Theory]
        [InlineData("To")]
        [InlineData("to")]
        public void BasicRegexTokenParser(string value)
        {
            var parser = new RegexJoiningWordParser();

            parser.IsMatch(value)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("To")]
        [InlineData("to")]
        public void FlexibleRegexTokenParser(string value)
        {
            var parser = new FlexibleRegexTokenParser(
                @"^[Tt]o$", 
                match => TokenisationResult.Success(new JoiningWord()));

            parser.IsMatch(value)
                .Should().BeTrue();
        }
    }

    public abstract class BaseRegexTokenParser : ITokenParser
    {
        Regex regex;
        
        public BaseRegexTokenParser(string pattern) =>
            regex = new Regex(pattern);

        public TokenisationResult Tokenise(string token)
        {
            var match = regex.Match(token);
            return match.Success ?
                convertMatch(match) :
                TokenisationResult.Fail();
        }

        protected abstract TokenisationResult convertMatch(Match match);
    }

    public class RegexJoiningWordParser : BaseRegexTokenParser
    {
        public RegexJoiningWordParser() 
            : base(@"^[Tt]o$") { }

        protected override TokenisationResult convertMatch(Match match) =>
            TokenisationResult.Success(new JoiningWord());
    }

    public class FlexibleRegexTokenParser : ITokenParser
    {
        Regex _regex;
        Func<Match, TokenisationResult> _converter;

        public FlexibleRegexTokenParser(string pattern, Func<Match, TokenisationResult> converter)
        {
            _regex = new Regex(pattern);
            _converter = converter;
        }

        public TokenisationResult Tokenise(string token)
        {
            var match = _regex.Match(token);
            return match.Success ?
                _converter(match) :
                TokenisationResult.Fail();
        }
    }
}
