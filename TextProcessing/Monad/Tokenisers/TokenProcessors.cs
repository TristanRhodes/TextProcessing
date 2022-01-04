using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Monad.Tokenisers
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

        public static TokenProcessor WeekDayDelegate = Tokeniser
            .FromRegex("^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$", 
            match =>
            {
                var day = DayGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => (DayOfWeek?)kvp.Value)
                    .SingleOrDefault();

                return (day is not null) ?
                    Token.Success(match.Value, day) :
                    Token.Fail(match.Value);
            });

        public static TokenProcessor Flags = Tokeniser
            .FromRegex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$", 
            match =>
            { 
                var flag = FlagGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => kvp.Value())
                    .SingleOrDefault();

                return (flag is not null) ?
                    Token.Success(match.Value, flag) :
                    Token.Fail(match.Value);
        }   );

        public static TokenProcessor JoiningWord = Tokeniser
            .FromRegex(@"^[Tt]o$", match =>
                Token.Success(match.Value, new JoiningWord()));

        public static TokenProcessor HypenSymbol = Tokeniser
            .FromChar('-', c => 
                Token.Success(c.ToString(), new HypenSymbol()));
        
        public static TokenProcessor Integer = (string token) =>
        {
            return int.TryParse(token, out int result) ? 
                Token.Success(token, result) : 
                Token.Fail(token);
        };

        public static TokenProcessor ClockTime = Tokeniser
            .FromRegex(@"^(((?<hr>[01]?\d|2[0-3]):(?<min>[0-5]\d|60))|((?<hr>([0]?\d)|1[0-2]):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))))?$", 
            match =>
            {
                var hour = int.Parse(match.Groups["hr"].Value);
                var min = int.Parse(match.Groups["min"].Value);
                var am = match.Groups["am"].Success;
                var pm = match.Groups["pm"].Success;
                var twentyFourHr = !am && !pm;

                if (twentyFourHr)
                {
                    return Token.Success(match.Value, new LocalTime(hour, min));
                }
                if (am)
                {
                    return Token.Success(match.Value, new LocalTime(hour == 12 ? 0 : hour, min));
                }
                else if (pm)
                {
                    return Token.Success(match.Value, new LocalTime(hour == 12 ? 12 : hour + 12, min));
                }

                return Token.Fail(match.Value);
            });
    }

    public static class Extensions
    {
        public static bool HasGroupMatch(this Match match, string groupName) =>
            match.Groups[groupName].Success;
    }
}
