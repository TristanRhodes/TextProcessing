using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Monad.Parsers
{
    public static class ExpressionParsers
    {
        public static Parser<DayTime> DayTimeParser =
           Parsers.Then(
               Parsers.IsToken<DayOfWeek>(),
               dow => Parsers.Select(
                   Parsers.IsToken<LocalTime>(),
                   lt => new DayTime { Day = dow, LocalTime = lt }));

        public static Parser<DayTime> DayTimeFluentParser =
            Parsers.IsToken<DayOfWeek>().Then(dow =>
                Parsers.IsToken<LocalTime>().Select(lt =>
                    new DayTime { Day = dow, LocalTime = lt }));

        public static Parser<DayTime> ExplicitDayTimeParser =
            DayTimeParser.End();

        public static Parser<RangeMarker> RangeMarker =
            Parsers.Or(
                Parsers.IsToken<HypenSymbol>().Select(r => new RangeMarker()),
                Parsers.IsToken<JoiningWord>().Select(r => new RangeMarker())
            );

        public static Parser<Range<DayOfWeek>> DayRangeParser =
            Parsers.IsToken<DayOfWeek>().Then(from =>
                RangeMarker.Then(_ =>
                    Parsers.IsToken<DayOfWeek>()
                        .Select(to => new Range<DayOfWeek> { From = from, To = to })));

        public static Parser<Range<LocalTime>> TimeRangeParser =
            Parsers.IsToken<LocalTime>().Then(from =>
                RangeMarker.Then(_ =>
                    Parsers.IsToken<LocalTime>()
                        .Select(to => new Range<LocalTime> { From = from, To = to })));

        public static Parser<OpenHours> OpenHoursParser =
            Parsers.IsToken<OpenFlag>().Then(_ =>
                DayRangeParser.Then(dr =>
                    TimeRangeParser.Select(tr => new OpenHours { Days = dr, Hours = tr })));

        public static Parser<List<LocalTime>> TourTimesParser =
            Parsers.IsToken<ToursFlag>().Then(_ =>
                Parsers.ListOf(
                    Parsers.IsToken<LocalTime>())
                        .Select(times => times));

        public static Parser<List<DayTime>> EventTimesParser =
            Parsers.IsToken<EventsFlag>().Then(_ =>
                Parsers.ListOf(DayTimeParser)
                    .Select(times => times));
    }
}
