using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.OO.Parsers
{
    public interface IParser<T>
    {
        ParseResult<T> Parse(Position position);
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

        public bool End => (Ordinal == Source.Length);

        public bool Beginning => Ordinal == 0;

        public Position Next() =>
            new Position(Source, Ordinal + 1);

        public static Position For(Token[] tokens) =>
            new Position(tokens, 0);
    }
}
