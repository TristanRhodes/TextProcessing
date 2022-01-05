using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OO.Tokenisers
{
    public class WeekDayTokenParser : ITokenParser
    {
        static Dictionary<string, DayOfWeek> DayGroupMap = new Dictionary<string, DayOfWeek>()
        {
            { "Monday", DayOfWeek.Monday },
            { "Tuesday", DayOfWeek.Tuesday },
            { "Wednesday", DayOfWeek.Wednesday },
            { "Thursday", DayOfWeek.Thursday },
            { "Friday", DayOfWeek.Friday },
            { "Saturday", DayOfWeek.Saturday },
            { "Sunday", DayOfWeek.Sunday },
        };

        Regex regex = new Regex("^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);

            if (!match.Success)
                return Token.Create(token);

            var day = DayGroupMap
                .Where(kvp => match.Groups[kvp.Key].Success)
                .Select(kvp => (DayOfWeek?)kvp.Value)
                .SingleOrDefault();

            return (day is not null) ?
                Token.Create(token, day) :
                throw new ArgumentException("Bad Pattern: " + token);
        }
    }

    public class JoiningWordTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^[Tt]o$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, new JoiningWord());
        }
    }

    public class FlagTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);

            if (match.Groups["pickup"].Success)
                return Token.Create(token, new PickupFlag());

            if (match.Groups["dropoff"].Success)
                return Token.Create(token, new DropoffFlag());

            if (match.Groups["open"].Success)
                return Token.Create(token, new OpenFlag());

            if (match.Groups["tours"].Success)
                return Token.Create(token, new ToursFlag());

            if (match.Groups["events"].Success)
                return Token.Create(token, new EventsFlag());

            throw new ApplicationException("No Match: " + token);
        }
    }

    public class IntegerTokenParser : ITokenParser
    {
        public bool IsMatch(string token)
        {
            return int.TryParse(token, out int _);
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, int.Parse(token));
        }
    }

    public class HypenSymbolTokenParser : ITokenParser
    {
        public bool IsMatch(string token)
        {
            return token == "-";
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, new HypenSymbol());
        }
    }

    public class ClockTimeTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^(((?<hr>[01]?\d|2[0-3]):(?<min>[0-5]\d|60))|((?<hr>([0]?\d)|1[0-2]):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))))?$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);
            if (!match.Success)
                throw new ArgumentException("Bad Pattern: " + token);

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"].Success;
            var pm = match.Groups["pm"].Success;
            var twentyFourHr = !am && !pm;

            if (twentyFourHr)
            {
                return Token.Create(token, new LocalTime(hour, min));
            }
            if (am)
            {
                return Token.Create(token, new LocalTime(hour == 12 ? 0 : hour, min));
            }
            else if (pm)
            {
                return Token.Create(token, new LocalTime(hour == 12 ? 12 : hour + 12, min));
            }

            throw new ArgumentException("Bad Pattern: " + token);
        }
    }
}
