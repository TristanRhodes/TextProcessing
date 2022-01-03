using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OOParsers
{
    public static class ParserExtensions
    {
        public static Parser<U> Then<T, U>(this Parser<T> core, Func<T, Parser<U>> then)
        {
            return new Then<T, U>(core, then);
        }

        public static Parser<U> Select<T, U>(this Parser<T> core, Func<T, U> select)
        {
            return new Select<T, U>(core, select);
        }
    }
}
