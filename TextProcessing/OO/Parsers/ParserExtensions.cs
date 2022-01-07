using System;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.OO.Parsers
{
    public static class ParserExtensions
    {
        public static IParser<U> Then<T, U>(this IParser<T> core, Func<T, IParser<U>> then) =>
            new Then<T, U>(core, then);

        public static IParser<U> Select<T, U>(this IParser<T> core, Func<T, U> select) =>
            new Select<T, U>(core, select);

        public static IParser<T> End<T>(this IParser<T> core) =>
            new End<T>(core);

        public static ParseResult<T> Parse<T>(this IParser<T> parser, Token[] tokens)
        {
            var position = Position.For(tokens);
            return parser.Parse(position);
        }
    }
}
