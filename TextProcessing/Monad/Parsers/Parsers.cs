using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.Monad.Parsers
{
    public static class Parsers
    {
        public static Parser<T> IsToken<T>() => (Position position) =>
        {
            if (!position.Current.Is<T>())
                return ParseResult<T>.Failure(position);

            return ParseResult<T>.Successful(position.Next(), position.Current.As<T>());
        };

        public static Parser<T> IsToken<T>(Func<T, bool> check) => (Position position) =>
        {
            if (!position.Current.Is<T>())
                return ParseResult<T>.Failure(position);

            if (!check(position.Current.As<T>()))
                return ParseResult<T>.Failure(position);

            return ParseResult<T>.Successful(position.Next(), position.Current.As<T>());
        };

        public static Parser<T> End<T>(Parser<T> child) => (Position position) =>
        {
            var result = child(position);
            if (!result.Success)
                ParseResult<T>.Failure(result.Position);

            return result.Position.End ?
                result :
                ParseResult<T>.Failure(result.Position);
        }; 
        
        public static Parser<List<T>> ListOf<T>(Parser<T> child) => (Position position) =>
        {
            var list = new List<T>();

            ParseResult<T> result;
            do
            {
                result = child(position);
                position = result.Position;

                if (result.Success)
                    list.Add(result.Value);

                if (result.Position.End)
                    break;

            } while (result.Success);

            return list.Any() ?
                ParseResult<List<T>>.Successful(position, list) :
                ParseResult<List<T>>.Failure(result.Position);
        };

        public static Parser<U> Select<T, U>(Parser<T> child, Func<T, U> converter) => (Position position) =>
        {
            var result = child(position);
            position = result.Position;

            return result.Success ?
                ParseResult<U>.Successful(position, converter(result.Value)) :
                ParseResult<U>.Failure(position);
        };

        public static Parser<U> Then<T, U>(Parser<T> child, Func<T, Parser<U>> then) => (Position position) =>
        {
            var result = child(position);
            position = result.Position;

            if (!result.Success)
                return ParseResult<U>.Failure(position);

            var thenParser = then(result.Value);
            var thenResult = thenParser(position);
            position = thenResult.Position;

            return thenResult.Success ?
                ParseResult<U>.Successful(position, thenResult.Value) :
                ParseResult<U>.Failure(position);
        };

        public static Parser<T> Or<T>(params Parser<T>[] children) => (Position position) =>
        {
            foreach (var parser in children)
            {
                var result = parser(position);
                if (result.Success)
                    return ParseResult<T>.Successful(result.Position, result.Value);
            }

            return ParseResult<T>.Failure(position);
        };
    }
}
