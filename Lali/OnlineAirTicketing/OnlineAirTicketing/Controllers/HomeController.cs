using DataAccessLayer;
using Models;
using Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineAirTicketing.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        UserDataAccess userDataAccess = new UserDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        AdminDataAccess adminDataAccess = new AdminDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult ValidateUser(UserDetail userDetail)
        {
            if (userDetail != null)
            {
                //encrypt the password 
                userDetail.password = RSAAlgo.EncryptText(userDetail.password);
                //validate the user with provided username and password
                int resultUserID = userDataAccess.validateUserInfo(userDetail.username, userDetail.password);
                UserDetail userTempDetail = userDataAccess.getUserInfo(resultUserID);

                if (resultUserID > 0)
                {
                    TempData["viewName"] = "updateInfo";
                    System.Web.HttpContext.Current.Session["userID"] = userTempDetail.userID;
                    System.Web.HttpContext.Current.Session["name"] = userTempDetail.personName;
                    return View("UserDashboard", userTempDetail);
                }
                else
                {
                    TempData["errorMessage"] = "Invalid Details, Please check details and submit again.";
                    return View("Index");
                }
            }
            else
            {
                TempData["errorMessage"] = "Invalid Details, Please check details and submit again.";
                return View("Index");
            }
            
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        public ActionResult Register()
        {
            UserDetail userDetail = new UserDetail();
            return View(userDetail);
        }

        /// <summary>
        /// Registers the specified user detail.
        /// </summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult Register(UserDetail userDetail)
        {
            //checks for each property if it is null or empty then show error message
            if (!string.IsNullOrEmpty(userDetail.password) &&
                    !string.IsNullOrEmpty(userDetail.confirmPassword) &&
                    !string.IsNullOrEmpty(userDetail.personName) &&
                    !string.IsNullOrEmpty(userDetail.contactNumber) &&
                    !string.IsNullOrEmpty(userDetail.address) &&
                    userDetail.password.Equals(userDetail.confirmPassword))
            {
                //encrypt the password before sending over network
                userDetail.password = RSAAlgo.EncryptText(userDetail.password);
                //add user to the system
                int userID = userDataAccess.addUser(userDetail);
                userDetail.userID = userID;
                //UserDetail userDetail = userDataAccess.getUserInfo(userID);
                if (userID > 0)
                {
                    TempData["viewName"] = "updateInfo";
                    System.Web.HttpContext.Current.Session["userID"] = userDetail.userID;
                    System.Web.HttpContext.Current.Session["name"] = userDetail.personName;
                    return View("UserDashboard", userDetail);
                }
                else
                {
                    userDetail.errorMsg = "Invalid Details, Please check details and submit again.";
                    return View(userDetail);
                }
            }
            else
            {
                userDetail.errorMsg = "Invalid Details, Please check details and submit again.";
                return View(userDetail);
            }
        }

        /// <summary>
        /// Forgets the password.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult ForgetPassword()
        {
            UserDetail userDetail = new UserDetail();
            return View(userDetail);
        }

        /// <summary>
        /// Retreives the sec question.
        /// </summary>
        /// <param name="tempUser">The temporary user.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult RetreiveSecQues(UserDetail tempUser)
        {
            if (tempUser != null)
            {
                //get the security question for the provider username
                UserDetail userDetail = userDataAccess.GetSecQuestion(tempUser.username);
                if (userDetail != null)
                {
                    Session["username"] = tempUser.username;
                    TempData["securityans"] = userDetail.securityAns;
                    return View("ForgetPassword", userDetail);
                }
                else
                {
                    userDetail = new UserDetail();
                    userDetail.errorMsg = "Username not found in the system.";
                    return View("ForgetPassword", userDetail);
                }
            }
            else
            {
                tempUser = new UserDetail();
                tempUser.errorMsg = "Please enter valid username.";
                return View("ForgetPassword", tempUser);
            }
            
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="tempUser">The temporary user.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult SetPassword(UserDetail tempUser)
        {
            if (tempUser != null)
            {
                //if user security answer matches then allow the user to change the password
                if (tempUser.securityAns.Equals(Convert.ToString(TempData["securityans"])))
                {
                    tempUser.username = Convert.ToString(Session["username"]);
                    //checks if user password and confirm password matches or not.
                    if (!string.IsNullOrEmpty(tempUser.password) && !string.IsNullOrEmpty(tempUser.confirmPassword) && tempUser.password.Equals(tempUser.confirmPassword))
                    {
                        tempUser.password = RSAAlgo.EncryptText(tempUser.password);
                        bool result = userDataAccess.setPassword(tempUser);
                        if (result)
                        {
                            UserDetail user = new UserDetail();
                            user.errorMsg = "Password changed successfully.";
                            return View("ForgetPassword", user);
                        }
                        else
                        {
                            UserDetail user = new UserDetail();

                            user.errorMsg = "Failed to change password.";
                            return View("ForgetPassword", user);
                        }
                    }
                    else
                    {
                        UserDetail user = new UserDetail();

                        user.errorMsg = "Invalid password/confirm password.";
                        return View("ForgetPassword", user);
                    }
                }
                else
                {
                    UserDetail user = new UserDetail();
                    user.errorMsg = "Invalid answer.";
                    
                    return View("ForgetPassword", user);
                }
            }
            else
            {
                UserDetail user = new UserDetail();
                user.errorMsg = "Please provide all details.";
                
                return View("ForgetPassword", user);
            }
        }

        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>JsonResult.</returns>
        [HttpGet]
        public JsonResult GetUserInfo(int userID)
        {
           

            UserDetail userTempDetail = userDataAccess.getUserInfo(userID);
            
            return Json(userTempDetail,JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Updates the user detail.
        /// </summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult UpdateUserDetail(UserDetail userDetail)
        {
            
            int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["userID"]);
            if (userID > 0)
            {
                userDetail.userID = userID;
                //check whether all the userdetails are provided or not
                if (!string.IsNullOrEmpty(userDetail.password) &&
                    !string.IsNullOrEmpty(userDetail.confirmPassword) &&
                    !string.IsNullOrEmpty(userDetail.personName) &&
                    !string.IsNullOrEmpty(userDetail.contactNumber) &&
                    !string.IsNullOrEmpty(userDetail.address) &&
                    userDetail.password.Equals(userDetail.confirmPassword))
                {
                    //if data are there, then update the user details
                    bool result = userDataAccess.updateUserDetail(userDetail);
                    if (result)
                    {
                        System.Web.HttpContext.Current.Session["name"] = userDetail.personName;
                        TempData["viewName"] = "updateInfo";
                        userDetail.errorMsg = "User details updated successfully.";

                    }
                    else
                    {
                        TempData["viewName"] = "updateInfo";
                        userDetail.errorMsg = "Data failed to update. Please try again.";

                    }
                    return View("UserDashboard", userDetail);
                }
                else
                {
                    TempData["viewName"] = "updateInfo";
                    userDetail.errorMsg = "Invalid details, Please check the data and submit again.";
                    return View("UserDashboard", userDetail);
                }
            }
            else
            {
                return View("Index");
            }
            
        }

        /// <summary>
        /// Searches the flight.
        /// </summary>
        /// <param name="travelDetail">The travel detail.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult SearchFlight(TravelDetail travelDetail)
        {
            //checks if seach flight details are there 
            if (travelDetail != null &&
                !string.IsNullOrEmpty(travelDetail.departDate) &&
                !string.IsNullOrEmpty(travelDetail.returnDate) &&
                !string.IsNullOrEmpty(travelDetail.To) &&
                !string.IsNullOrEmpty(travelDetail.From))
            {
                //if there set the details in session
                System.Web.HttpContext.Current.Session["departDate"] = travelDetail.departDate;
                System.Web.HttpContext.Current.Session["returnDate"] = travelDetail.returnDate;
                System.Web.HttpContext.Current.Session["to"] = travelDetail.To;
                System.Web.HttpContext.Current.Session["from"] = travelDetail.From;
                return View();

            }
            else
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["userID"])))
                {
                    UserDetail userDetail = new UserDetail();
                    TempData["viewName"] = "searchFlight";
                    userDetail.errorMsg = "Invalid Details, Please check details and submit again.";
                    return View("UserDashboard", userDetail);
                }
                else
                {
                    TempData["errorMessage"] = "Invalid Details, Please check details and submit again.";
                    return View("Index");
                }
                
            }
            
        }

        /// <summary>
        /// Gets the flights for depart.
        /// </summary>
        /// <returns>JsonResult.</returns>
        public JsonResult GetFlightsForDepart()
        {

            string to = Convert.ToString(System.Web.HttpContext.Current.Session["to"]);
            string departDate = Convert.ToString(System.Web.HttpContext.Current.Session["departDate"]);
            string from = Convert.ToString(System.Web.HttpContext.Current.Session["from"]);
            List<FlightDetail> flightDetailList  = null;
             List<FlightDetail> filteredFlightDetails  = null;
            if (!string.IsNullOrEmpty(to)
                && !string.IsNullOrEmpty(from)
                && !string.IsNullOrEmpty(departDate))
            {
                //get all the flights
               flightDetailList  = adminDataAccess.GetFlights();
               filteredFlightDetails = new List<FlightDetail>();
                //retrieve the flights that match the departing flight criteria
                foreach (FlightDetail flightDetail in flightDetailList)
                {
                    if (flightDetail.origin.Trim().Equals(from.Trim(), StringComparison.InvariantCultureIgnoreCase)
                        && flightDetail.destination.Trim().Equals(to.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<FlightLegDetail> flightLegList = adminDataAccess.GetFlightLegs(Convert.ToInt32(flightDetail.flightID));
                        if (flightLegList != null && flightLegList.Count == flightDetail.noOfLegs)
                        {
                            flightDetail.departTime = flightLegList[0].departTime;
                            flightDetail.arrivalTime = flightLegList[flightDetail.noOfLegs - 1].arrivalTime;
                        }
                        flightDetail.flightLegs = flightLegList;
                        filteredFlightDetails.Add(flightDetail);
                    }
                }
            }
            return Json(filteredFlightDetails,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the flights for return.
        /// </summary>
        /// <returns>JsonResult.</returns>
        public JsonResult GetFlightsForReturn()
        {
            string to = Convert.ToString(System.Web.HttpContext.Current.Session["to"]);
            string departDate = Convert.ToString(System.Web.HttpContext.Current.Session["departDate"]);
            string from = Convert.ToString(System.Web.HttpContext.Current.Session["from"]);
            string returnDate = Convert.ToString(System.Web.HttpContext.Current.Session["returnDate"]);
            List<FlightDetail> flightDetailList = null;
            List<FlightDetail> filteredFlightDetails = null;
            if (!string.IsNullOrEmpty(to)
                && !string.IsNullOrEmpty(from)
                && !string.IsNullOrEmpty(departDate)
                && !string.IsNullOrEmpty(returnDate))
            {
                //get all the flights
                flightDetailList = adminDataAccess.GetFlights();
                filteredFlightDetails = new List<FlightDetail>();
                //retrieve the flights that match the return flight criteria
                foreach (FlightDetail flightDetail in flightDetailList)
                {
                    if (flightDetail.origin.Trim().Equals(to.Trim(), StringComparison.InvariantCultureIgnoreCase)
                        && flightDetail.destination.Trim().Equals(from.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<FlightLegDetail> flightLegList = adminDataAccess.GetFlightLegs(Convert.ToInt32(flightDetail.flightID));
                        if (flightLegList != null && flightLegList.Count == flightDetail.noOfLegs)
                        {
                            flightDetail.departTime = flightLegList[0].departTime;
                            flightDetail.arrivalTime = flightLegList[flightDetail.noOfLegs - 1].arrivalTime;
                        }
                        flightDetail.flightLegs = flightLegList;
                        filteredFlightDetails.Add(flightDetail);
                    }
                }
            }
            return Json(filteredFlightDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GoToHome()
        {
            UserDetail userDetail = new UserDetail();
            TempData["viewName"] = "searchFlight";
            return View("UserDashboard", userDetail);
        }

        /// <summary>
        /// Makes the booking.
        /// </summary>
        /// <param name="departureFlight">The departure flight.</param>
        /// <param name="returnFlight">The return flight.</param>
        /// <returns>JsonResult.</returns>
        public JsonResult MakeBooking(BookingData departureFlight, BookingData returnFlight)
        {
            string departDate = Convert.ToString(System.Web.HttpContext.Current.Session["departDate"]);
            string returnDate = Convert.ToString(System.Web.HttpContext.Current.Session["returnDate"]);
            int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["userID"]);
            departureFlight.travelDate = departDate;
            returnFlight.travelDate = returnDate;
            string pnrno = string.Empty;
            int ageInNumber = 0;
            bool result = false;
            if (departureFlight != null && !string.IsNullOrEmpty(departureFlight.passengerName)
                && !string.IsNullOrEmpty(departureFlight.contactnumber)
                && Int32.TryParse(departureFlight.age, out ageInNumber))
            {
                result = userDataAccess.MakeBooking(departureFlight, returnFlight, userID, out pnrno);
            }
            return Json(new { res = result, pnr = pnrno }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the bookings for user.
        /// </summary>
        /// <returns>JsonResult.</returns>
        public JsonResult GetBookingsForUser()
        {
            int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["userID"]);
            List<BookingData> bookingDetails = userDataAccess.GetBookingDetailsForUser(userID);
            List<FlightDetail> flights = adminDataAccess.GetFlights();
            int count = 0;
            List<UserFlights> userFlights = new List<UserFlights>();
            string previouspnr = string.Empty;
            UserFlights userFlight = null;
            foreach (BookingData bookingData in bookingDetails)
            {
                
                FlightDetail tempFlightDetail = null;
                foreach (FlightDetail flightDetail in flights)
                {
                    if (Convert.ToInt32(flightDetail.flightID) == bookingData.flightID)
                    {
                        tempFlightDetail = flightDetail;
                        break;
                    }
                }
                List<FlightLegDetail> flightLegList = adminDataAccess.GetFlightLegs(Convert.ToInt32(tempFlightDetail.flightID));
                if (flightLegList != null && flightLegList.Count == tempFlightDetail.noOfLegs)
                    {
                        bookingData.departTime = flightLegList[0].departTime;
                        bookingData.arrivalTime = flightLegList[tempFlightDetail.noOfLegs - 1].arrivalTime;
                    }
                bookingData.from = tempFlightDetail.origin;
                bookingData.to = tempFlightDetail.destination;
                
                if (count % 2 == 0)
                {
                    userFlight = new UserFlights();
                    userFlight.departureFlight = bookingData;
                }
                else
                {
                    if (userFlight != null)
                    {
                        userFlight.returnFlight = bookingData;
                        userFlights.Add(userFlight);
                    }
                }
                count++;
            }
            return Json(userFlights, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="pnrno">The pnrno.</param>
        /// <returns>JsonResult.</returns>
        public JsonResult CancelBooking(string pnrno)
        {
            bool result = userDataAccess.cancelTicket(pnrno);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            System.Web.HttpContext.Current.Session.RemoveAll();
            return View("Index");
        }
    }
}
