using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    /// <summary>
    /// Class FlightDetail.
    /// </summary>
    public class FlightDetail
    {
        /// <summary>
        /// Gets or sets the flight number.
        /// </summary>
        /// <value>The flight number.</value>
        public string flightNumber { get; set; }
        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public string origin { get; set;}
        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public string destination { get; set; }
        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public float distance { get; set; }
        /// <summary>
        /// Gets or sets the no of legs.
        /// </summary>
        /// <value>The no of legs.</value>
        public int noOfLegs { get; set; }
        /// <summary>
        /// Gets or sets the flight legs.
        /// </summary>
        /// <value>The flight legs.</value>
        public List<FlightLegDetail> flightLegs { get; set; }
        /// <summary>
        /// Gets or sets the flight identifier.
        /// </summary>
        /// <value>The flight identifier.</value>
        public string flightID { get; set; }
        /// <summary>
        /// Gets or sets the arrival time.
        /// </summary>
        /// <value>The arrival time.</value>
        public string arrivalTime { get; set; }
        /// <summary>
        /// Gets or sets the depart time.
        /// </summary>
        /// <value>The depart time.</value>
        public string departTime { get; set; }
    }
}