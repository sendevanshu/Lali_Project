using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class UserDetail
    {
       
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set;}
        public string personName { get; set;}
        public string contactNumber { get; set; }
        public string address { get; set; }
        public string gender { get; set; }
        public string securityQues { get; set; }
        public string securityAns { get; set; }
        public string errorMsg { get; set;}
        public int personID { get; set; }
        public int userID { get; set; }
    }
}
