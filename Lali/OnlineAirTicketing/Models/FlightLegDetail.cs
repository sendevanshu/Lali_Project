using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class FlightLegDetail
    {
        public string flightlegNo { get; set; }
        public string duration { get; set; }
        public string departTime { get; set; }
        public string arrivalTime { get; set; }
        public float baseFare { get; set; }
        public string departingAirport { get; set; }
        public string arrivalAirport { get; set; }
        public string legOrigin { get; set; }
        public string legDestination { get; set; }

        public string flightLegID { get; set; }
    }
}
