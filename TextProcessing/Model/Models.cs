using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.Model
{
    public class PickupFlag { }

    public class DropoffFlag { }

    public class OpenFlag { }

    public class ToursFlag { }

    public class EventsFlag { }

    public class HypenSymbol { }

    public class JoiningWord { }

    public class RangeMarker { }

    public class DayTime
    {
        public DayTime()
        {
        }

        public DayTime(DayOfWeek day, LocalTime time)
        {
            Day = day;
            LocalTime = time;
        }

        public DayOfWeek Day { get; set; }
        public LocalTime LocalTime { get; set; }
    }

    public class PickupDropoff
    {
        public DayTime Pickup { get; set; }

        public DayTime DropOff { get; set; }
    }

    public record Range<T>
    {
        public T From { get; init; }
        public T To { get; init; }
    }

    public record OpenHours
    {
        public Range<DayOfWeek> Days { get; init; }
        public Range<LocalTime> Hours { get; init; }
    }
}
