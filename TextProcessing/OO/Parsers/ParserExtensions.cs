using System;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.OO.Parsers
{
    public static class ParserExtensions
    {
        public static Parser<U> Then<T, U>(this Parser<T> core, Func<T, Parser<U>> then) =>
            new Then<T, U>(core, then);

        public static Parser<U> Select<T, U>(this Parser<T> core, Func<T, U> select) =>
            new Select<T, U>(core, select);

        public static Parser<T> End<T>(this Parser<T> core) =>
            new End<T>(core);

        public static ParseResult<T> Parse<T>(this Parser<T> parser, Token[] tokens)
        {
            var position = Position.For(tokens);
            return parser.Parse(position);
        }
    }
}
