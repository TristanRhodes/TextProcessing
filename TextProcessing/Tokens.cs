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
    public class Unknown : Token
    {
        public Unknown(string value)
            : base(value) { }
    }

    public class Day : Token
    {
        public Day(string value, DayOfWeek dayOfWeek)
            : base(value) 
        {
            DayOfWeek = dayOfWeek;
        }

        public DayOfWeek DayOfWeek { get; }
    }

    public class Time : Token
    {
        public Time(string value, LocalTime localTime)
            : base(value)
        {
            LocalTime = localTime;
        }

        public LocalTime LocalTime { get; set; }
    }

    public class To : Token
    {
        public To(string value)
            : base(value) { }
    }

    public class Hyphen : Token
    {
        public Hyphen(string value)
            : base(value) { }
    }
}
