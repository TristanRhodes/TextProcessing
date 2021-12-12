using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class ClockTimeTokeniser : ITokeniser
    {
        Regex regex = new Regex(@"^(?<hr>[01]?\d|2[0-4]):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))?$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);
            if (!match.Success)
                return null;

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"].Success;
            var pm = match.Groups["pm"].Success;
            var twentyFourHr = !am && !pm;

            if (twentyFourHr)
            {
                return new TimeToken(token, new LocalTime(hour, min));
            }
            if (am)
            {
                return new TimeToken(token, new LocalTime(hour == 12 ? 0 : hour, min));
            }
            else if (pm)
            {
                return new TimeToken(token, new LocalTime(hour == 12 ? 12 : hour + 12, min));
            }

            throw new NotSupportedException($"Bad Format: {token}");
        }
    }
}
