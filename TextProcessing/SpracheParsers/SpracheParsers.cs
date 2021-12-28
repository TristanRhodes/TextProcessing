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
    // Separate two part element context
    //"Pickup Mon 08:00 dropoff wed 17:00"

    // Range elements
    //"Open Mon to Fri 08:00 - 18:00"

    // Repeating tokens
    //"Tours 10:00 12:00 14:00 17:00 20:00"

    // Repeating complex elements
    //"Events Tuesday 18:00 Wednesday 15:00 Friday 12:00"

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

    public class ExpressionParsers
    {
        public static Parser<DayTime> DayTime =
            from day in TokenParsers.DayOfWeek
            from _ in Parse.WhiteSpace
            from time in TokenParsers.LocalTime
            select new DayTime(day, time);

        public static Parser<DayTime> DayTimeDelegate =
            TokenParsers.DayOfWeek.Then(day =>
                Parse.WhiteSpace.Then(_ =>
                    TokenParsers.LocalTime.Select(time =>
                        new DayTime(day, time))));

        public static Parser<DayTime> PickupTime =
            from pickupFlag in TokenParsers.PickupFlag
            from _ in Parse.WhiteSpace
            from pickup in DayTime
            select pickup;

        public static Parser<DayTime> DropOffTime =
            from dropOffFlag in TokenParsers.DropOffFlag
            from _ in Parse.WhiteSpace
            from pickupFlag in DayTime
            select pickupFlag;

        public static Parser<PickupDropoff> PickupDropOff =
            from pickup in PickupTime
            from _ in Parse.WhiteSpace
            from dropOff in DropOffTime
            select new PickupDropoff { Pickup = pickup, DropOff = dropOff };

        public static Parser<Range<DayOfWeek>> DayRange =
            from start in TokenParsers.DayOfWeek
            from _ in TokenParsers.RangeMarker
            from end in TokenParsers.DayOfWeek
            select new Range<DayOfWeek> { From = start, To = end };

        public static Parser<Range<LocalTime>> TimeRange =
            from start in TokenParsers.LocalTime
            from _ in TokenParsers.RangeMarker
            from end in TokenParsers.LocalTime
            select new Range<LocalTime> { From = start, To = end };

        public static Parser<OpenHours> OpenHours =
            from start in TokenParsers.OpenFlag
            from _1 in Parse.WhiteSpace
            from days in DayRange
            from _2 in Parse.WhiteSpace
            from hours in TimeRange
            select new OpenHours { Days = days, Hours = hours};

        public static Parser<IEnumerable<LocalTime>> TourTimes =
            from toursFlag in TokenParsers.ToursFlag
            from _ in Parse.WhiteSpace
            from times in Parse.DelimitedBy(TokenParsers.LocalTime, Parse.WhiteSpace)
            select times;

        public static Parser<IEnumerable<DayTime>> EventSchedule =
            from eventsFlag in TokenParsers.EventsFlag
            from _ in Parse.WhiteSpace
            from times in Parse.DelimitedBy(DayTime, Parse.WhiteSpace)
            select times;
    }
}
