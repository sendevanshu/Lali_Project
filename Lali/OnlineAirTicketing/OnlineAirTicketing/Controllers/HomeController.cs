using DataAccessLayer;
using Models;
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
                bool result = userDataAccess.updateUserDetail(userDetail);
                if (result)
                {
                    System.Web.HttpContext.Current.Session["name"] = userDetail.personName;
                    userDetail.errorMsg = "User details updated successfully.";

                }
                else
                {
                    userDetail.errorMsg = "Data failed to update. Please try again.";

                }
                return View("UserDashboard", userDetail);
            }
            else
            {
                return View("Index");
            }
            
        }

        public ActionResult SearchFlight(TravelDetail travelDetail)
        {
            if (travelDetail != null)
            {
                System.Web.HttpContext.Current.Session["departDate"] = travelDetail.departDate;
                System.Web.HttpContext.Current.Session["returnDate"] = travelDetail.returnDate;
                System.Web.HttpContext.Current.Session["to"] = travelDetail.To;
                System.Web.HttpContext.Current.Session["from"] = travelDetail.From;
                
            }
            return View();
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
    }
}
