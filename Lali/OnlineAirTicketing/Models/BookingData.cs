using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    /// <summary>
    /// Class BookingData.
    /// </summary>
    public class BookingData
    {
        /// <summary>
        /// Gets or sets the name of the passenger.
        /// </summary>
        /// <value>The name of the passenger.</value>
        public string passengerName { get; set; }
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        public string age { get; set; }
        /// <summary>
        /// Gets or sets the contactnumber.
        /// </summary>
        /// <value>The contactnumber.</value>
        public string contactnumber { get; set; }
        /// <summary>
        /// Gets or sets the flight identifier.
        /// </summary>
        /// <value>The flight identifier.</value>
        public int flightID { get; set; }
        /// <summary>
        /// Gets or sets the flight leg identifier.
        /// </summary>
        /// <value>The flight leg identifier.</value>
        public List<int> flightLegID { get; set; }
        /// <summary>
        /// Gets or sets the travel date.
        /// </summary>
        /// <value>The travel date.</value>
        public string travelDate { get; set; }
        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>The cost.</value>
        public string cost { get; set; }
        /// <summary>
        /// Gets or sets the pnrno.
        /// </summary>
        /// <value>The pnrno.</value>
        public string pnrno { get; set; }
        /// <summary>
        /// Gets or sets the seatno.
        /// </summary>
        /// <value>The seatno.</value>
        public List<int> seatno { get; set; }
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public double amount { get; set; }
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string from { get; set; }
        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public string to { get; set; }
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
    }
}
