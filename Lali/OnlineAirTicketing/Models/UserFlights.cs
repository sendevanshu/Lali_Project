using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    /// <summary>
    /// Class UserFlights.
    /// </summary>
    public class UserFlights
    {
        /// <summary>
        /// Gets or sets the departure flight.
        /// </summary>
        /// <value>The departure flight.</value>
        public BookingData departureFlight { get; set; }
        /// <summary>
        /// Gets or sets the return flight.
        /// </summary>
        /// <value>The return flight.</value>
        public BookingData returnFlight { get; set; }
    }
}
