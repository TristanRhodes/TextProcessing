using NodaTime;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessing.Model;

namespace TextProcessing.SpracheParsers
{
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
