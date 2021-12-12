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
        public Token(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() =>
            $"{this.GetType().Name}: {Value}";
    }

    /// <summary>
    /// Represents an Unknown token value.
    /// </summary>
    public class UnknownToken : Token
    {
        public UnknownToken(string value)
            : base(value) { }
    }

    public class DayToken : Token
    {
        public DayToken(string value, DayOfWeek dayOfWeek)
            : base(value) 
        {
            DayOfWeek = dayOfWeek;
        }

        public DayOfWeek DayOfWeek { get; }
    }

    public class TimeToken : Token
    {
        public TimeToken(string value, LocalTime localTime)
            : base(value)
        {
            LocalTime = localTime;
        }

        public LocalTime LocalTime { get; set; }
    }

    /// <summary>
    /// Represents a recognised word
    /// </summary>
    public class WordToken : Token
    {
        public WordToken(string value)
            : base(value) { }
    }

    /// <summary>
    /// Represents a recognised symbol
    /// </summary>
    public class SymbolToken : Token
    {
        public SymbolToken(string value)
            : base(value) { }
    }
}
