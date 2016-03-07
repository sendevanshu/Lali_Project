using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class BookingData
    {
        public string passengerName { get; set; }
        public string age { get; set; }
        public string contactnumber { get; set; }
        public int flightID { get; set; }
        public List<int> flightLegID { get; set; }
        public string travelDate { get; set; }
        public string cost { get; set; }
        public string pnrno { get; set; }
    }
}
