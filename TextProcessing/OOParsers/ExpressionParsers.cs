using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OOParsers
{
    public static class ExpressionParsers
    {
        public static Parser<DayTime> DayTimeParser =
           new Then<DayOfWeek, DayTime>(
               new IsToken<DayOfWeek>(),
               dow => new Select<LocalTime, DayTime>(
                   new IsToken<LocalTime>(),
                   lt => new DayTime { Day = dow, LocalTime = lt }));

        public static Parser<DayTime> DayTimeFluentParser =
            Parser.IsToken<DayOfWeek>().Then(dow =>
                Parser.IsToken<LocalTime>().Select(lt =>
                    new DayTime { Day = dow, LocalTime = lt }));

        public static Parser<DayTime> ExplicitDayTimeParser =
            DayTimeParser.End();

        public static Parser<RangeMarker> RangeMarker =
            Parser.Or(
                Parser.IsToken<HypenSymbol>().Select(r => new RangeMarker()),
                Parser.IsToken<JoiningWord>().Select(r => new RangeMarker())
            );

        public static Parser<Range<DayOfWeek>> DayRangeParser =
            Parser.IsToken<DayOfWeek>().Then(from =>
                RangeMarker.Then(_ =>
                    Parser.IsToken<DayOfWeek>()
                        .Select(to => new Range<DayOfWeek> { From = from, To = to })));

        public static Parser<Range<LocalTime>> TimeRangeParser =
            Parser.IsToken<LocalTime>().Then(from =>
                RangeMarker.Then(_ =>
                    Parser.IsToken<LocalTime>()
                        .Select(to => new Range<LocalTime> { From = from, To = to })));

        public static Parser<OpenHours> OpenHoursParser =
            Parser.IsToken<OpenFlag>().Then(_ =>
                DayRangeParser.Then(dr =>
                    TimeRangeParser.Select(tr => new OpenHours { Days = dr, Hours = tr })));

        public static Parser<List<LocalTime>> TourTimesParser =
            Parser.IsToken<ToursFlag>().Then(_ =>
                Parser.ListOf(
                    Parser.IsToken<LocalTime>())
                        .Select(times => times));

        public static Parser<List<DayTime>> EventTimesParser =
            Parser.IsToken<EventsFlag>().Then(_ =>
                Parser.ListOf(DayTimeParser)
                    .Select(times => times));
    }
}
