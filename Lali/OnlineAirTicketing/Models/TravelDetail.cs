using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    /// <summary>
    /// Class TravelDetail.
    /// </summary>
    public class TravelDetail
    {
        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public string To { get; set; }
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string From { get; set; }
        /// <summary>
        /// Gets or sets the depart date.
        /// </summary>
        /// <value>The depart date.</value>
        public string departDate { get; set; }
        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        /// <value>The return date.</value>
        public string returnDate { get; set; }
    }
}
