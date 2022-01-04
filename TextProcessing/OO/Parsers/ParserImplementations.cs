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
    public class IsToken<T> : Parser<T>
    {
        private Func<T, bool> check;
        public IsToken() { }
        public IsToken(Func<T, bool> check) => 
            this.check = check;

        public override ParseResult<T> Parse(Position position)
        {
            if (!position.Current.Is<T>())
                return ParseResult<T>.Failure(position);

            if (check != null && !check(position.Current.As<T>()))
                return ParseResult<T>.Failure(position);

            return ParseResult<T>.Successful(position.Next(), position.Current.As<T>());
        }
    }

    public class End<T> : Parser<T>
    {
        Parser<T> _core;

        public End(Parser<T> parser) =>
            _core = parser;

        public override ParseResult<T> Parse(Position position)
        {
            var result = _core.Parse(position);
            if (!result.Success)
                ParseResult<T>.Failure(result.Position);

            return result.Position.End ?
                result :
                ParseResult<T>.Failure(result.Position);
        }
    }

    public class ListOf<T> : Parser<List<T>>
    {
        Parser<T> _core;

        public ListOf(Parser<T> parser) =>
            _core = parser;

        public override ParseResult<List<T>> Parse(Position position)
        {
            var list = new List<T>();

            ParseResult<T> result;
            do
            {
                result = _core.Parse(position);
                position = result.Position;

                if (result.Success)
                    list.Add(result.Value);

                if (result.Position.End)
                    break;

            } while (result.Success);

            return list.Any() ?
                ParseResult<List<T>>.Successful(position, list) :
                ParseResult<List<T>>.Failure(result.Position);
        }
    }

    public class Select<T, U> : Parser<U>
    {
        Parser<T> _core;
        Func<T, U> _converter;

        public Select(Parser<T> core, Func<T, U> converter)
        {
            _core = core;
            _converter = converter;
        }

        public override ParseResult<U> Parse(Position position)
        {
            var result = _core.Parse(position);
            position = result.Position;

            return result.Success ?
                ParseResult<U>.Successful(position, _converter(result.Value)) :
                ParseResult<U>.Failure(position);
        }
    }

    public class Then<T, U> : Parser<U>
    {
        Parser<T> _core;
        Func<T, Parser<U>> _second;

        public Then(Parser<T> core, Func<T, Parser<U>> second)
        {
            _core = core;
            _second = second;
        }

        public override ParseResult<U> Parse(Position position)
        {
            var result = _core.Parse(position);
            position = result.Position;

            if (!result.Success)
                return ParseResult<U>.Failure(position);

            var thenResult = _second(result.Value)
                .Parse(position);
            position = thenResult.Position;

            return thenResult.Success ?
                ParseResult<U>.Successful(position, thenResult.Value) :
                ParseResult<U>.Failure(position);
        }
    }

    public class Or<T> : Parser<T>
    {
        Parser<T>[] _options;

        public Or(params Parser<T>[] options) => _options = options;

        public override ParseResult<T> Parse(Position position)
        {
            foreach(var parser in _options)
            {
                var result = parser.Parse(position);
                if (result.Success)
                    return ParseResult<T>.Successful(result.Position, result.Value);
            }

            return ParseResult<T>.Failure(position);
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
