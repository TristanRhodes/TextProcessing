using NodaTime;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.SpracheParsers
{

    public class TokenParsers
    {
        public static Parser<LocalTime> LocalTime =
            Parse
                .RegexMatch(@"(((?<hr>([01]\d)|2[0-3]|^\d):(?<min>[0-5]\d|60))|((?<hr>([0]\d)|1[0-2]|^\d):(?<min>[0-5]\d|60)((?<am>am)|(?<pm>pm))))\b")
                .Select(match =>
                {
                    var hour = int.Parse(match.Groups["hr"].Value);
                    var min = int.Parse(match.Groups["min"].Value);
                    var am = match.Groups["am"].Success;
                    var pm = match.Groups["pm"].Success;
                    var twentyFourHr = !am && !pm;

                    if (twentyFourHr)
                    {
                        return new LocalTime(hour, min);
                    }
                    if (am)
                    {
                        return new LocalTime(hour == 12 ? 0 : hour, min);
                    }
                    else if (pm)
                    {
                        return new LocalTime(hour == 12 ? 12 : hour + 12, min);
                    }

                    throw new ArgumentException("Bad Pattern: " + match);
                });

        public static Parser<DayOfWeek> DayOfWeek =
            Parse
                .RegexMatch("(?<Monday>[Mm]on(day)?)|(?<Tuesday>[Tt]ue(sday)?)|(?<Wednesday>[Ww]ed(nesday)?)|(?<Thursday>[Tt]hu(rs(day)?)?)|(?<Friday>[Ff]ri(day)?)|(?<Saturday>[Ss]at(urday)?)|(?<Sunday>[Ss]un(day)?)")
                .Select(match =>
                {
                    if (match.Groups["Monday"].Success)
                        return System.DayOfWeek.Monday;

                    if (match.Groups["Tuesday"].Success)
                        return System.DayOfWeek.Tuesday;

                    if (match.Groups["Wednesday"].Success)
                        return System.DayOfWeek.Wednesday;

                    if (match.Groups["Thursday"].Success)
                        return System.DayOfWeek.Thursday;

                    if (match.Groups["Friday"].Success)
                        return System.DayOfWeek.Friday;

                    if (match.Groups["Saturday"].Success)
                        return System.DayOfWeek.Saturday;

                    if (match.Groups["Sunday"].Success)
                        return System.DayOfWeek.Sunday;

                    throw new ArgumentException("Bad Pattern: " + match);
                });

        public static Parser<PickupFlag> PickupFlag = 
            Parse
                .RegexMatch(@"([Pp]ickup)")
                .Select(match => new PickupFlag());

        public static Parser<DropoffFlag> DropOffFlag = 
            Parse
                .RegexMatch(@"[Dd]ropoff")
                .Select(match => new DropoffFlag());

        public static Parser<OpenFlag> OpenFlag = 
            Parse
                .RegexMatch(@"[Oo]pen")
                .Select(match => new OpenFlag());

        public static Parser<ToursFlag> ToursFlag = 
            Parse
                .RegexMatch(@"[Tt]ours")
                .Select(match => new ToursFlag());

        public static Parser<EventsFlag> EventsFlag = 
            Parse
                .RegexMatch(@"[Ee]vents")
                .Select(match => new EventsFlag());

        public static Parser<RangeMarker> RangeMarker = new List<Parser<RangeMarker>>
        {
               Parse.Regex(" ?- ?").Select(c => new RangeMarker()),
               Parse.Regex(" [Tt]o ").Select(s => new RangeMarker())
        }.Aggregate((a, b) => a.Or(b));
    }
}
