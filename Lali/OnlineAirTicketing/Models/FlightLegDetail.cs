using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    /// <summary>
    /// Class FlightLegDetail.
    /// </summary>
    public class FlightLegDetail
    {
        /// <summary>
        /// Gets or sets the flightleg no.
        /// </summary>
        /// <value>The flightleg no.</value>
        public string flightlegNo { get; set; }
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public string duration { get; set; }
        /// <summary>
        /// Gets or sets the depart time.
        /// </summary>
        /// <value>The depart time.</value>
        public string departTime { get; set; }
        /// <summary>
        /// Gets or sets the arrival time.
        /// </summary>
        /// <value>The arrival time.</value>
        public string arrivalTime { get; set; }
        /// <summary>
        /// Gets or sets the base fare.
        /// </summary>
        /// <value>The base fare.</value>
        public float baseFare { get; set; }
        /// <summary>
        /// Gets or sets the departing airport.
        /// </summary>
        /// <value>The departing airport.</value>
        public string departingAirport { get; set; }
        /// <summary>
        /// Gets or sets the arrival airport.
        /// </summary>
        /// <value>The arrival airport.</value>
        public string arrivalAirport { get; set; }
        /// <summary>
        /// Gets or sets the leg origin.
        /// </summary>
        /// <value>The leg origin.</value>
        public string legOrigin { get; set; }
        /// <summary>
        /// Gets or sets the leg destination.
        /// </summary>
        /// <value>The leg destination.</value>
        public string legDestination { get; set; }

        /// <summary>
        /// Gets or sets the flight leg identifier.
        /// </summary>
        /// <value>The flight leg identifier.</value>
        public string flightLegID { get; set; }
    }
}
