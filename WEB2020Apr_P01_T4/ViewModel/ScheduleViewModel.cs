﻿using System;
using System.Collections.Generic;
using WEB2020Apr_P01_T4.Models;

namespace WEB2020Apr_P01_T4.ViewModel
{
    public class ScheduleViewModel
    {
        public FlightSchedule FlightSchedule { get; set; }
        public Route CreateRoute { get; set; }
        public List<String> SearchOption { get; set; }
<<<<<<< HEAD
        public IEnumerable<FlightSchedule> FlightScheduleList { get; set; }
        public bool ShowEditPop { get; set; }
        public int TicketSize { get; set; }

=======
        public List<FlightSchedule> FlightScheduleList { get; set; }
        public bool ShowEditPop { get; set; }
        public int TicketSize { get; set; }



>>>>>>> Scheduling
    }
}
