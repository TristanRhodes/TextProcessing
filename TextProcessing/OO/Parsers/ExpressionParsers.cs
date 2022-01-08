using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OO.Parsers
{
    public static class ExpressionParsers
    {
        public static IParser<DayTime> DayTimeParser =
           new Then<DayOfWeek, DayTime>(
               new IsToken<DayOfWeek>(),
               dow => new Select<LocalTime, DayTime>(
                   new IsToken<LocalTime>(),
                   lt => new DayTime { Day = dow, LocalTime = lt }));

        public static IParser<DayTime> DayTimeFluentParser =
            Parsers.IsToken<DayOfWeek>().Then(dow =>
                Parsers.IsToken<LocalTime>().Select(lt =>
                    new DayTime { Day = dow, LocalTime = lt }));

        public static IParser<DayTime> ExplicitDayTimeParser =
            DayTimeParser.End();

        public static IParser<RangeMarker> RangeMarker =
            Parsers.Or(
                Parsers.IsToken<HypenSymbol>().Select(r => new RangeMarker()),
                Parsers.IsToken<JoiningWord>().Select(r => new RangeMarker())
            );

        public static IParser<Range<DayOfWeek>> DayRangeParser =
            Parsers.IsToken<DayOfWeek>().Then(from =>
                RangeMarker.Then(_ =>
                    Parsers.IsToken<DayOfWeek>()
                        .Select(to => new Range<DayOfWeek> { From = from, To = to })));

        public static IParser<Range<LocalTime>> TimeRangeParser =
            Parsers.IsToken<LocalTime>().Then(from =>
                RangeMarker.Then(_ =>
                    Parsers.IsToken<LocalTime>()
                        .Select(to => new Range<LocalTime> { From = from, To = to })));

        public static IParser<OpenHours> OpenHoursParser =
            Parsers.IsToken<OpenFlag>().Then(_ =>
                DayRangeParser.Then(dr =>
                    TimeRangeParser.Select(tr => new OpenHours { Days = dr, Hours = tr })));

        public static IParser<List<LocalTime>> TourTimesParser =
            Parsers.IsToken<ToursFlag>().Then(_ =>
                Parsers.ListOf(
                    Parsers.IsToken<LocalTime>())
                        .Select(times => times));

        public static IParser<List<DayTime>> EventTimesParser =
            Parsers.IsToken<EventsFlag>().Then(_ =>
                Parsers.ListOf(DayTimeParser)
                    .Select(times => times));

        public static IParser<DayTime> PickupDayTime = Parsers
            .IsToken<PickupFlag>()
            .Then(_ => DayTimeParser);

        public static IParser<DayTime> DropOffDayTime = Parsers
            .IsToken<DropoffFlag>()
            .Then(_ => DayTimeParser);

        public static IParser<PickupDropoff> PickupDropOff = PickupDayTime
            .Then(pu => DropOffDayTime
            .Select(dr => new PickupDropoff { Pickup = pu, DropOff = dr }))
            .End();
    }
}
