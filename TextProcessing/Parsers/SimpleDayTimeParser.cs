using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Parsers
{
    public class SimpleDayTimeParser
    {
        public bool IsMatch(Token[] tokens)
        {
            if (tokens.Length != 2) return false;
            if (!tokens[0].Is<DayOfWeek>()) return false;
            if (!tokens[1].Is<LocalTime>()) return false;
            return true;
        }

        public DayTime Parse(Token[] tokens)
        {
            if (!IsMatch(tokens))
                throw new ApplicationException("Bad Match");

            return new DayTime(
                tokens[0].As<DayOfWeek>(),
                tokens[1].As<LocalTime>());
        }
    }
}
