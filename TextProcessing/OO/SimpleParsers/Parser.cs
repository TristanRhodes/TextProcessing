using System;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.OO.SimpleParsers
{
    public abstract class Parser<T>
    {
        public abstract ParseResult<T> Parse(Token[] tokens);
    }

    public class ParseResult<T>
    {
        T _value;

        private ParseResult()
        {
            Success = false;
            _value = default(T);
        }

        private ParseResult(T result)
        {
            Success = true;
            _value = result;
        }

        public bool Success { get; }

        public T Value => Success ? _value : throw new ArgumentException("Not Successful");

        public static ParseResult<T> Successful(T t) =>
            new ParseResult<T>(t);

        public static ParseResult<T> Failure() =>
            new ParseResult<T>();
    }
}
