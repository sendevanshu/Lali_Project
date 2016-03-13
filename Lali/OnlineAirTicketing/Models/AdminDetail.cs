using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    /// <summary>
    /// This class is used to get and set details of Admin
    /// </summary>
    public class AdminDetail
    {
        /// <summary>
        /// username of Admin
        /// </summary>
        public string username { get; set;}
        /// <summary>
        /// password of Admin
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// property to contain error message
        /// </summary>
        public string errorMsg { get; set; }
 
    }
}