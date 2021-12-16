using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class DayTime
    {
        public DayTime(DayOfWeek day, LocalTime time)
        {
            Day = day;
            LocalTime = time;
        }

        public DayOfWeek Day { get; }
        public LocalTime LocalTime { get; }
    }
}
