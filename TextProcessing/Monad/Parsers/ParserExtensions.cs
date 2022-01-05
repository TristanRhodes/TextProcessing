using System;
using TextProcessing.Monad.Tokenisers;

namespace TextProcessing.Monad.Parsers
{
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
}
