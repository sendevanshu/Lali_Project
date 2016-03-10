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

        public ActionResult ValidateUser(UserDetail userDetail)
        {
            if (userDetail != null)
            {
                userDetail.password = RSAAlgo.EncryptText(userDetail.password);
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

        [HttpGet]
        public ActionResult Register()
        {
            UserDetail userDetail = new UserDetail();
            return View(userDetail);
        }

        [HttpPost]
        public ActionResult Register(UserDetail userDetail)
        {
            
            userDetail.password = RSAAlgo.EncryptText(userDetail.password);
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

        public ActionResult ForgetPassword()
        {
            UserDetail userDetail = new UserDetail();
            return View(userDetail);
        }

        public ActionResult RetreiveSecQues(UserDetail tempUser)
        {
            if (tempUser != null)
            {
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

        public ActionResult SetPassword(UserDetail tempUser)
        {
            if (tempUser != null)
            {
                if (tempUser.securityAns.Equals(Convert.ToString(TempData["securityans"])))
                {
                    tempUser.username = Convert.ToString(Session["username"]);
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

        [HttpGet]
        public JsonResult GetUserInfo(int userID)
        {
           

            UserDetail userTempDetail = userDataAccess.getUserInfo(userID);
            
            return Json(userTempDetail,JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateUserDetail(UserDetail userDetail)
        {
            
            int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["userID"]);
            if (userID > 0)
            {
                userDetail.userID = userID;
                if (!string.IsNullOrEmpty(userDetail.password) &&
                    !string.IsNullOrEmpty(userDetail.confirmPassword) &&
                    !string.IsNullOrEmpty(userDetail.personName) &&
                    !string.IsNullOrEmpty(userDetail.contactNumber) &&
                    !string.IsNullOrEmpty(userDetail.address) &&
                    userDetail.password.Equals(userDetail.confirmPassword))
                {
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

        public ActionResult SearchFlight(TravelDetail travelDetail)
        {
            if (travelDetail != null &&
                !string.IsNullOrEmpty(travelDetail.departDate) &&
                !string.IsNullOrEmpty(travelDetail.returnDate) &&
                !string.IsNullOrEmpty(travelDetail.To) &&
                !string.IsNullOrEmpty(travelDetail.From))
            {
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
               flightDetailList  = adminDataAccess.GetFlights();
               filteredFlightDetails = new List<FlightDetail>();
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
                flightDetailList = adminDataAccess.GetFlights();
                filteredFlightDetails = new List<FlightDetail>();
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

        public JsonResult MakeBooking(BookingData departureFlight, BookingData returnFlight)
        {
            string departDate = Convert.ToString(System.Web.HttpContext.Current.Session["departDate"]);
            string returnDate = Convert.ToString(System.Web.HttpContext.Current.Session["returnDate"]);
            int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["userID"]);
            departureFlight.travelDate = departDate;
            returnFlight.travelDate = returnDate;
            string pnrno = string.Empty;
            bool result = userDataAccess.makeBooking(departureFlight, returnFlight, userID, out pnrno);
            return Json(new { res = result, pnr = pnrno }, JsonRequestBehavior.AllowGet);
        }


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
