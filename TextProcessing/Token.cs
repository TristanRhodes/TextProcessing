using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    public class Token
    {
        protected Token(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; }

        public object Value { get; }

        public static Token Create(string text) =>
                new Token(text, text);

        public static Token Create(string text, object obj) =>
                new Token(text, obj);

        public bool Is<T>() =>
            Value is T;

        public T As<T>() =>
            (T)Value;

        public override string ToString() =>
            $"{this.GetType().Name}: {Value}";
    }
}
