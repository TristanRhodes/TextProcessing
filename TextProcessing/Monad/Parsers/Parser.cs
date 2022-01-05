using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Monad.Tokenisers;

namespace TextProcessing.Monad.Parsers
{
    public delegate ParseResult<T> Parser<T>(Position position);

    public static class ParserExtensions
    {
        public static Parser<U> Then<T, U>(this Parser<T> core, Func<T, Parser<U>> then) =>
            Parsers.Then(core, then);

        public static Parser<U> Select<T, U>(this Parser<T> core, Func<T, U> select) =>
            Parsers.Select(core, select);

        public static Parser<T> End<T>(this Parser<T> core) =>
            Parsers.End(core);

        public static ParseResult<T> Parse<T>(this Parser<T> parser, Token[] tokens)
        {
            var position = Position.For(tokens);
            return parser(position);
        }
    }

    public class ParseResult<T>
    {
        T _value;

        private ParseResult(Position position)
        {
            Position = position;
            Success = false;
            _value = default(T);
        }

        private ParseResult(Position position, T result)
        {
            Position = position;
            Success = true;
            _value = result;
        }

        public bool Success { get; }
        public Position Position { get; }

        public T Value => Success ? _value : throw new ArgumentException("Not Successful");

        public static ParseResult<T> Successful(Position position, T t) =>
            new ParseResult<T>(position, t);

        public static ParseResult<T> Failure(Position position) =>
            new ParseResult<T>(position);
    }

    public class Position
    {
        public Position(Token[] source, int ordinal)
        {
            Source = source;
            Ordinal = ordinal;
        }

        public Token[] Source { get; }

        public int Ordinal { get; }

        public Token Current => !End ? Source[Ordinal] : throw new ApplicationException("At End");

        public bool End { get; set; } = false;

        public bool Beginning => Ordinal == 0;

        public Position Next()
        {
            if (Ordinal == Source.Length - 1)
                return new Position(Source, Source.Length) { End = true };

            return new Position(Source, Ordinal + 1);
        }

        public static Position For(Token[] tokens) =>
            new Position(tokens, 0);
    }
}
