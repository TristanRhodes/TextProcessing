using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OOParsers
{
    public static class Parser
    {
        public static Parser<T> IsToken<T>() =>
            new IsToken<T>();

        public static Parser<T> Select<T>(Func<T> selector) =>
            new Select<T>(selector);
    }

    public static class ParserExtensions
    {
        public static Parser<U> Then<T, U>(this Parser<T> core, Func<T, Parser<U>> then) =>
            new Then<T, U>(core, then);

        public static Parser<U> Select<T, U>(this Parser<T> core, Func<T, U> select) =>
            new Select<T, U>(core, select);

        public static Parser<T> End<T>(this Parser<T> core) =>
            new End<T>(core);
    }
}
