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
        public Token(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    public class Token<T> : Token
    {
        public Token(string text, T value) 
            : base(text)
        {
            Value = value;
        }

        public T Value { get; }

        public override string ToString() =>
            $"{this.GetType().Name}: {Value}";
    }
}
