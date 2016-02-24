using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class FlightDetail
    {
        public string flightNumber { get; set; }
        public string origin { get; set;}
        public string destination { get; set; }
        public float distance { get; set; }
        public int noOfLegs { get; set; }
        public List<FlightLegDetail> flightLegs { get; set; }
        public string flightID { get; set; }
        public string arrivalTime { get; set; }
        public string departTime { get; set; }
    }
}