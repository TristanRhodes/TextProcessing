using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class DayTokeniser : ITokeniser
    {
        Regex regex = new Regex("^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);
            if (!match.Success)
                throw new NotSupportedException($"Bad Format: {token}");

            if (match.Groups["Monday"].Success)
                return new Day(token, DayOfWeek.Monday);

            if (match.Groups["Tuesday"].Success)
                return new Day(token, DayOfWeek.Tuesday);

            if (match.Groups["Wednesday"].Success)
                return new Day(token, DayOfWeek.Wednesday);

            if (match.Groups["Thursday"].Success)
                return new Day(token, DayOfWeek.Thursday);

            if (match.Groups["Friday"].Success)
                return new Day(token, DayOfWeek.Friday);

            if (match.Groups["Saturday"].Success)
                return new Day(token, DayOfWeek.Saturday);

            if (match.Groups["Sunday"].Success)
                return new Day(token, DayOfWeek.Sunday);

            throw new NotSupportedException($"Bad Format: {token}");
        }
    }
}
