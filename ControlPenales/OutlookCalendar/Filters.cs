using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ControlPenales
{
    public static class Filters
    {
        public static IEnumerable<Appointment> ByDate(this IEnumerable<Appointment> appointments, DateTime date)
        {
            if (appointments != null)
            {
                var app = from a in appointments
                          where a.StartTime.ToShortDateString() == date.ToShortDateString()
                          select a;
                return app;
            }
            return null;
        }
    }
}
