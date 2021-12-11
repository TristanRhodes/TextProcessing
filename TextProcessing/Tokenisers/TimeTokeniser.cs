using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class TimeTokeniser : ITokeniser
    {
        string regex = @"^(?<hr>\d{1,2}):(?<min>\d{2})((?<am>am)|(?<pm>pm))?$";

        public bool IsMatch(string token)
        {
            return Regex.IsMatch(token, regex);
        }

        public Token Tokenise(string token)
        {
            var match = Regex.Match(token, regex);
            if (!match.Success)
                return null;

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"].Success;
            var pm = match.Groups["pm"].Success;
            var twentyFourHr = !am && !pm;

            if (min > 60)
                throw new NotSupportedException($"Bad Format: {token}");

            if (twentyFourHr && hour < 24)
            {
                return new Time(token, new LocalTime(hour, min));
            }
            else if (am && hour < 12)
            {
                return new Time(token, new LocalTime(hour, min));
            }
            else if (pm && hour < 12)
            {
                return new Time(token, new LocalTime(hour + 12, min));
            }

            throw new NotSupportedException($"Bad Format: {token}");
        }
    }
}
