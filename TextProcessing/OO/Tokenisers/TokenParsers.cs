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

        public TokenisationResult Tokenise(string token)
        {
            var match = regex.Match(token);
            if (!match.Success)
                return TokenisationResult.Fail();

            var day = DayGroupMap
                .Where(kvp => match.Groups[kvp.Key].Success)
                .Select(kvp => (DayOfWeek?)kvp.Value)
                .SingleOrDefault();

            return (day is not null) ?
                TokenisationResult.Success(day) :
                TokenisationResult.Fail();
        }
    }

    public class JoiningWordTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^[Tt]o$");

        public TokenisationResult Tokenise(string token)
        {
            return regex.IsMatch(token) ? 
                TokenisationResult.Success(new JoiningWord()) :
                TokenisationResult.Fail();
        }
    }

    public class FlagTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$");

        public TokenisationResult Tokenise(string token)
        {
            var match = regex.Match(token);

            if (match.Groups["pickup"].Success)
                return TokenisationResult.Success(new PickupFlag());

            if (match.Groups["dropoff"].Success)
                return TokenisationResult.Success(new DropoffFlag());

            if (match.Groups["open"].Success)
                return TokenisationResult.Success(new OpenFlag());

            if (match.Groups["tours"].Success)
                return TokenisationResult.Success(new ToursFlag());

            if (match.Groups["events"].Success)
                return TokenisationResult.Success(new EventsFlag());

            return TokenisationResult.Fail();
        }
    }

    public class IntegerTokenParser : ITokenParser
    {
        public TokenisationResult Tokenise(string token)
        {
            return int.TryParse(token, out int i) ?
                TokenisationResult.Success(i) :
                TokenisationResult.Fail();
        }
    }

    public class HypenSymbolTokenParser : ITokenParser
    {
        public TokenisationResult Tokenise(string token)
        {
            return token == "-" ?
                TokenisationResult.Success(new HypenSymbol()) :
                TokenisationResult.Fail();
        }
    }

    public class ClockTimeTokenParser : ITokenParser
    {
        Regex regex = new Regex(@"^(((?<hr>[01]?\d|2[0-3]):(?<min>[0-5]\d|60))|((?<hr>([0]?\d)|1[0-2]):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))))?$");

        public TokenisationResult Tokenise(string token)
        {
            var match = regex.Match(token);
            if (!match.Success)
                return TokenisationResult.Fail();

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"].Success;
            var pm = match.Groups["pm"].Success;
            var twentyFourHr = !am && !pm;

            if (twentyFourHr)
            {
                return TokenisationResult.Success(new LocalTime(hour, min));
            }
            if (am)
            {
                return TokenisationResult.Success(new LocalTime(hour == 12 ? 0 : hour, min));
            }
            else if (pm)
            {
                return TokenisationResult.Success(new LocalTime(hour == 12 ? 12 : hour + 12, min));
            }

            return TokenisationResult.Fail();
        }
    }
}
