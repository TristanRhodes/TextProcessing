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

    public class RangeMarker { }

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
