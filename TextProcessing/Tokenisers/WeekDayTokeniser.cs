using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class WeekDayTokeniser : ITokeniser
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
                throw new ArgumentException("Bad Pattern: " + token);

            if (match.Groups["Monday"].Success)
                return Token.Create(token, DayOfWeek.Monday);

            if (match.Groups["Tuesday"].Success)
                return Token.Create(token, DayOfWeek.Tuesday);

            if (match.Groups["Wednesday"].Success)
                return Token.Create(token, DayOfWeek.Wednesday);

            if (match.Groups["Thursday"].Success)
                return Token.Create(token, DayOfWeek.Thursday);

            if (match.Groups["Friday"].Success)
                return Token.Create(token, DayOfWeek.Friday);

            if (match.Groups["Saturday"].Success)
                return Token.Create(token, DayOfWeek.Saturday);

            if (match.Groups["Sunday"].Success)
                return Token.Create(token, DayOfWeek.Sunday);

            throw new ArgumentException("Bad Pattern: " + token);
        }
    }
}
