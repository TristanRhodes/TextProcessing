using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.Model
{
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
}
