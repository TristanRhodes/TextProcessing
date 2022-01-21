using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Functional.Tokenisers
{
    public static class CommonTokenParsers
    {
        public static TokenParser HypenSymbol = Tokeniser
            .FromChar('-', c =>
                TokenisationResult.Success(new HypenSymbol()));

        public static TokenParser Integer = (string token) =>
        {
            return int.TryParse(token, out int result) ?
                TokenisationResult.Success(result) :
                TokenisationResult.Fail();
        };

        public static TokenParser ClockTime = Tokeniser
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
            });
    }

    public static class EnglishTokenParsers
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

        public static TokenParser WeekDay = Tokeniser
            .FromRegex("^(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)$",
            match =>
            {
                var day = DayGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => (DayOfWeek?)kvp.Value)
                    .SingleOrDefault();

                return (day is not null) ?
                    TokenisationResult.Success(day) :
                    TokenisationResult.Fail();
            });

        public static TokenParser Flags = Tokeniser
            .FromRegex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$",
            match =>
            {
                var flag = FlagGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => kvp.Value())
                    .SingleOrDefault();

                return (flag is not null) ?
                    TokenisationResult.Success(flag) :
                    TokenisationResult.Fail();
            });

        public static TokenParser JoiningWord = Tokeniser
            .FromRegex(@"^[Tt]o$", match =>
                TokenisationResult.Success(new JoiningWord()));
    }

    public static class GermanTokenParsers
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

        public static TokenParser WeekDay = Tokeniser
            .FromRegex("^(?<Monday>[Mm]on(tag)?)|(?<Tuesday>[Dd]ie(nstag)?)|(?<Wednesday>[Mm]it(twoch)?)|(?<Thursday>[Dd]on(erstag)?)|(?<Friday>[Ff]rei(tag)?)|(?<Saturday>[Ss]ams(tag)?)|(?<Sunday>[Ss]onn(tag)?)$",
            match =>
            {
                var day = DayGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => (DayOfWeek?)kvp.Value)
                    .SingleOrDefault();

                return (day is not null) ?
                    TokenisationResult.Success(day) :
                    TokenisationResult.Fail();
            });

        public static TokenParser Flags = Tokeniser
            .FromRegex(@"^(?<pickup>[Aa]ufsammeln)|(?<dropoff>[Aa]blegen)|(?<open>[Oo]ffen)|(?<tours>[Tt]ouren)|(?<events>[Vv]eranstaltungen)$",
            match =>
            {
                var flag = FlagGroupMap
                    .Where(kvp => match.HasGroupMatch(kvp.Key))
                    .Select(kvp => kvp.Value())
                    .SingleOrDefault();

                return (flag is not null) ?
                    TokenisationResult.Success(flag) :
                    TokenisationResult.Fail();
            });

        public static TokenParser JoiningWord = Tokeniser
            .FromRegex(@"^[Bb]is$", match =>
                TokenisationResult.Success(new JoiningWord()));
    }
}
