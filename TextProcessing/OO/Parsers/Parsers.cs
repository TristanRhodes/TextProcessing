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
    public static class Parsers
    {
        public static IParser<T> IsToken<T>() =>
            new IsToken<T>();

        public static IParser<List<T>> ListOf<T>(IParser<T> parser) =>
            new ListOf<T>(parser);

        public static IParser<T> Or<T>(params IParser<T>[] options) =>
            new Or<T>(options);
    }

    public class IsToken<T> : IParser<T>
    {
        private Func<T, bool> check;
        public IsToken() { }
        public IsToken(Func<T, bool> check) => 
            this.check = check;

        public ParseResult<T> Parse(Position position)
        {
            if (!position.Current.Is<T>())
                return ParseResult<T>.Failure(position);

            if (check != null && !check(position.Current.As<T>()))
                return ParseResult<T>.Failure(position);

            return ParseResult<T>.Successful(position.Next(), position.Current.As<T>());
        }
    }

    public class End<T> : IParser<T>
    {
        IParser<T> _core;

        public End(IParser<T> parser) =>
            _core = parser;

        public ParseResult<T> Parse(Position position)
        {
            var result = _core.Parse(position);
            if (!result.Success)
                ParseResult<T>.Failure(result.Position);

            return result.Position.End ?
                result :
                ParseResult<T>.Failure(result.Position);
        }
    }

    public class ListOf<T> : IParser<List<T>>
    {
        IParser<T> _core;

        public ListOf(IParser<T> parser) =>
            _core = parser;

        public ParseResult<List<T>> Parse(Position position)
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

    public class Select<T, U> : IParser<U>
    {
        IParser<T> _core;
        Func<T, U> _converter;

        public Select(IParser<T> core, Func<T, U> converter)
        {
            _core = core;
            _converter = converter;
        }

        public ParseResult<U> Parse(Position position)
        {
            var result = _core.Parse(position);
            position = result.Position;

            return result.Success ?
                ParseResult<U>.Successful(position, _converter(result.Value)) :
                ParseResult<U>.Failure(position);
        }
    }

    public class Then<T, U> : IParser<U>
    {
        IParser<T> _core;
        Func<T, IParser<U>> _second;

        public Then(IParser<T> core, Func<T, IParser<U>> second)
        {
            _core = core;
            _second = second;
        }

        public ParseResult<U> Parse(Position position)
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

    public class Or<T> : IParser<T>
    {
        IParser<T>[] _options;

        public Or(params IParser<T>[] options) => _options = options;

        public ParseResult<T> Parse(Position position)
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
}
