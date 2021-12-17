using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;
using TextProcessing.Tokenisers;

namespace TextProcessing
{
    public abstract class Parser<T>
    {
        public abstract ParseResult<T> Parse(Position position);

        public ParseResult<T> Parse(Token[] tokens)
        {
            var position = Position.For(tokens);
            return Parse(position);
        }
    }

    public class Beginning<T> : Parser<T>
    {
        Parser<T> _core;

        public Beginning(Parser<T> parser) =>
            _core = parser;

        public override ParseResult<T> Parse(Position position)
        {
            if (!position.Beginning)
                return ParseResult<T>.Failure(position);

            return _core.Parse(position);
        }
    }

    public class Match<T> : Parser<T>
    {
        public override ParseResult<T> Parse(Position position)
        {
            return position.Current.Is<T>() ?
                ParseResult<T>.Successful(position, position.Current.As<T>()) :
                ParseResult<T>.Failure(position);
        }
    }

    public class End<T> : Parser<T>
    {
        Parser<T> _core;

        public End(Parser<T> parser) =>
            _core = parser;

        public override ParseResult<T> Parse(Position position)
        {
            if (!position.End)
                return ParseResult<T>.Failure(position);

            return new Match<T>().Parse(position);
        }
    }

    public class DayTimeParser : Parser<DayTime>
    {
        Parser<DayOfWeek> DayOfWeekParser = new Beginning<DayOfWeek>(new Match<DayOfWeek>());
        Parser<LocalTime> LocalTimeParser = new End<LocalTime>(new Match<LocalTime>());

        public override ParseResult<DayTime> Parse(Position position)
        {
            var dow = DayOfWeekParser.Parse(position);
            if (!dow.Success)
                return ParseResult<DayTime>.Failure(dow.Position);

            var localTime = LocalTimeParser.Parse(dow.Position.Next());
            if (!localTime.Success)
                return ParseResult<DayTime>.Failure(localTime.Position);

            var result = new DayTime(dow.Value, localTime.Value);

            return ParseResult<DayTime>.Successful(localTime.Position, result);
        }
    }

    public class ParseResult<T>
    {
        T _value = default(T);

        private ParseResult(Position position)
        {
            Position = position;
            Success = false;
        }

        private ParseResult(Position position, T result)
        {
            Position = position;
            Success = true;
            _value = result;
        }

        public bool Success { get; }

        public T Value => Success ? _value : throw new ArgumentException("Not Successfult");

        public Position Position { get; }

        public static ParseResult<T> Successful(Position position, T t)
        {
            return new ParseResult<T>(position, t);
        }

        public static ParseResult<T> Failure(Position position)
        {
            return new ParseResult<T>(position);
        }
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

        public Token Current => Source[Ordinal];

        public bool End => Ordinal == Source.Length - 1;

        public bool Beginning => Ordinal == 0;

        public Position Next()
        {
            if (End)
                throw new ApplicationException("At End");

            return new Position(Source, Ordinal + 1);
        }

        public static Position For(Token[] tokens) =>
            new Position(tokens, 0);
    }
}
