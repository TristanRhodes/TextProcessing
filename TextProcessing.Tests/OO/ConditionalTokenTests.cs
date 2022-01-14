using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.OO.Parsers;
using TextProcessing.OO.Tokenisers;
using Xunit;

namespace TextProcessing.Tests.OO
{
    public class ConditionalTokenTests
    {
        [Theory]
        [InlineData("1", false)]
        [InlineData("10", false)]
        [InlineData("101", true)]
        [InlineData("1000", true)]
        public void ConcreteParser(string text, bool succeed)
        {
            var tokens = Tokenise(text);
            new NumberOverParser(100)
                .Parse(tokens)
                .Success
                .Should().Be(succeed);
        }

        [Theory]
        [InlineData("1", false)]
        [InlineData("10", false)]
        [InlineData("101", true)]
        [InlineData("1000", true)]
        public void FlexibleParser(string text, bool succeed)
        {
            var tokens = Tokenise(text);
            new FlexibleIsTokenCondition<int>(i => i > 100)
                .Parse(tokens)
                .Success
                .Should().Be(succeed);
        }

        public static Token[] Tokenise(string text, bool fullMatch = false)
        {
            var processor = new Tokeniser(" ",
                new IntegerTokenParser());

            var tokens = processor
                .Tokenise(text)
                .ToArray();

            if (fullMatch && tokens.Any(t => t.Is<string>()))
                throw new ApplicationException("Unmatched tokens.");

            return tokens;
        }
    }

    public abstract class IsTokenConditionBase<T> : IParser<T>
    {
        public ParseResult<T> Parse(Position position)
        {
            return position.Current.Is<T>() && check(position.Current.As<T>()) ?
                ParseResult<T>.Successful(position.Next(), position.Current.As<T>()) :
                ParseResult<T>.Failure(position);
        }

        public abstract bool check(T entity);
    }

    public class NumberOverParser : IsTokenConditionBase<int>
    {
        int _value;

        public NumberOverParser(int value) =>
            _value = value;

        public override bool check(int entity) =>
            entity > _value;
    }

    public class FlexibleIsTokenCondition<T> : IParser<T>
    {
        Func<T, bool> _check;

        public FlexibleIsTokenCondition(Func<T, bool> check) =>
            _check = check;

        public ParseResult<T> Parse(Position position)
        {
            return position.Current.Is<T>() && _check(position.Current.As<T>()) ?
                ParseResult<T>.Successful(position.Next(), position.Current.As<T>()) :
                ParseResult<T>.Failure(position);
        }
    }
}
