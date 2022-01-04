using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.MonadTokenisers
{
    public static class TokenProcessors
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

        static Dictionary<string, Func<object>> FlagGroupMap = new Dictionary<string, Func<object>>()
        {
            { "pickup", () => new PickupFlag() },
            { "dropoff", () => new DropoffFlag()},
            { "open", () => new OpenFlag() },
            { "tours", () => new ToursFlag() },
            { "events", () => new EventsFlag() }
        };

        public static TokenProcessor WeekDay = (string token) =>
        {
            Regex regex = new Regex("^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$");

            var match = regex.Match(token);

            if (!match.Success)
                return Token.Fail(token);

            var day = DayGroupMap
                .Where(kvp => match.HasGroupMatch(kvp.Key))
                .Select(kvp => (DayOfWeek?)kvp.Value)
                .SingleOrDefault();

            return (day is not null) ? 
                Token.Success(token, day) :
                Token.Fail(token);
        };

        public static TokenProcessor Flags = (string token) =>
        {
            Regex regex = new Regex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$");

            var match = regex.Match(token);

            if (!match.Success)
                return Token.Fail(token);

            var flag = FlagGroupMap
                .Where(kvp => match.HasGroupMatch(kvp.Key))
                .Select(kvp => kvp.Value())
                .SingleOrDefault();

            return (flag is not null) ?
                Token.Success(token, flag) :
                Token.Fail(token);
        };

        public static TokenProcessor JoiningWord = (string token) =>
        {
            Regex regex = new Regex(@"^[Tt]o$");

            return regex.IsMatch(token) ?
                Token.Success(token, new JoiningWord()) :
                Token.Fail(token);
        };

        public static TokenProcessor HypenSymbol = (string token) =>
        {
            return (token == "-") ? 
                Token.Success(token, new HypenSymbol()) :
                Token.Fail(token);
        }; 
        
        public static TokenProcessor Integer = (string token) =>
        {
            return int.TryParse(token, out int result) ? 
                Token.Success(token, result) : 
                Token.Fail(token);
        };

        public static TokenProcessor ClockTime = (string token) =>
        {
            Regex regex = new Regex(@"^(((?<hr>[01]?\d|2[0-3]):(?<min>[0-5]\d|60))|((?<hr>([0]?\d)|1[0-2]):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))))?$");

            var match = regex.Match(token);
            if (!match.Success)
                return Token.Fail(token);

            var hour = int.Parse(match.Groups["hr"].Value);
            var min = int.Parse(match.Groups["min"].Value);
            var am = match.Groups["am"].Success;
            var pm = match.Groups["pm"].Success;
            var twentyFourHr = !am && !pm;

            if (twentyFourHr)
            {
                return Token.Success(token, new LocalTime(hour, min));
            }
            if (am)
            {
                return Token.Success(token, new LocalTime(hour == 12 ? 0 : hour, min));
            }
            else if (pm)
            {
                return Token.Success(token, new LocalTime(hour == 12 ? 12 : hour + 12, min));
            }

            return Token.Fail(token);
        };
    }

    public static class Extensions
    {
        public static bool HasGroupMatch(this Match match, string groupName) =>
            match.Groups[groupName].Success;
    }
}
