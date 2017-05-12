using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    public class Appointments : ObservableCollection<Appointment>
    {
        public Appointments()
        {
            Add(new Appointment() { Subject = "Dummy Appointment #1", StartTime = new DateTime(2015, 12, 2, 8, 00, 00), EndTime = new DateTime(2015, 12, 2, 9, 00, 00) });
            Add(new Appointment() { Subject = "Dummy Appointment #2", StartTime = new DateTime(2015, 12, 2, 9, 00, 00), EndTime = new DateTime(2015, 12, 2, 10, 00, 00) });
            Add(new Appointment() { Subject = "Dummy Appointment #3", StartTime = new DateTime(2015, 12, 2, 10, 00, 00), EndTime = new DateTime(2015, 12, 2, 11, 00, 00) });
        }
    }
}
