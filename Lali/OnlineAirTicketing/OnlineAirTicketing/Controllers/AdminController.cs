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
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        UserDataAccess userDataAccess = new UserDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        AdminDataAccess adminDataAccess = new AdminDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        public ActionResult AdminPage()
        {
            AdminDetail adminDetail = new AdminDetail();
            adminDetail.errorMsg = string.Empty;
            return View(adminDetail);
        }

        public ActionResult ValidateAdmin(AdminDetail adminDetail)
        {
            string viewName = string.Empty;
            bool isLoginSuccessful = adminDataAccess.validateAdminCred(adminDetail.username, adminDetail.password);
            if (!isLoginSuccessful)
            {
                adminDetail.errorMsg = "Invalid Username/Password";
                return View("AdminPage", adminDetail);
            }
            return View("AdminDashboard");
        }

        public JsonResult AddFlight(FlightDetail flightDetail)
        {
            int flightID = adminDataAccess.AddFlight(flightDetail);
            return Json(new { flightID = flightID },JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetFlights()
        {
            List<FlightDetail> flightList = adminDataAccess.GetFlights();
            foreach (FlightDetail flight in flightList)
            {
                List<FlightLegDetail> flightLegList = adminDataAccess.GetFlightLegs(Convert.ToInt32(flight.flightID));
                if (flightLegList != null && flightLegList.Count == flight.noOfLegs)
                {
                    flight.departTime = flightLegList[0].departTime;
                    flight.arrivalTime = flightLegList[flight.noOfLegs - 1].arrivalTime;
                }
            }
            return Json(flightList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFlight(int flightID)
        {
            bool res = adminDataAccess.DeleteFlight(flightID);
            if (res)
            {
                return Json(new { returnCode = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { returnCode = -1 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
