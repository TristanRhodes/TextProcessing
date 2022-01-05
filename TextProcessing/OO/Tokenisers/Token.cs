using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.OO.Tokenisers
{
    public class Token
    {
        protected Token(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public static Token Create(object obj) =>
                new Token(obj);

        public bool Is<T>() =>
            Value is T;

        public T As<T>() =>
            (T)Value;

        public override string ToString() =>
            $"{this.GetType().Name}: {Value}";
    }
}
