using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases
{
    public class ToolsTimer
    {
        /// <summary>
        /// Returns the period of time left before the specified hour is due to elapse
        /// </summary>
        /// <param name="hour">And integer representing an hour, 
        /// where 0 is midnight, 12 is midday, 23 is eleven etcetera</param>
        /// <returns>A TimeSpan representing the calculated time period</returns>
        public static TimeSpan GetTimeUntilNextHour(int hour, int minutes, int seconds)
        {
            var currentTime = Fechas.GetFechaDateServer;
            var desiredTime = new DateTime(Fechas.GetFechaDateServer.Year,
                Fechas.GetFechaDateServer.Month, Fechas.GetFechaDateServer.Day, hour, minutes, seconds);
            var timeDifference = (currentTime - desiredTime);
            var timePeriod = currentTime.Hour >= hour ?
                (desiredTime.AddDays(1) - currentTime) : -timeDifference;
            return timePeriod;
        }
    }
}
