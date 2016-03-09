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
            if (adminDetail != null)
            {
                adminDetail.password = RSAAlgo.EncryptText(adminDetail.password);
                bool isLoginSuccessful = adminDataAccess.validateAdminCred(adminDetail.username, adminDetail.password);
                if (!isLoginSuccessful)
                {
                    adminDetail.errorMsg = "Invalid Username/Password";
                    return View("AdminPage", adminDetail);
                }
                System.Web.HttpContext.Current.Session["username"] = adminDetail.username;
                return View("AdminDashboard");
            }
            else
            {
                adminDetail.errorMsg = "Please provide details";
                return View("AdminPage", adminDetail);
            }
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

        public JsonResult ModifyFlight(FlightDetail flightDetail)
        {
            List<FlightLegDetail> flightLegList = adminDataAccess.GetFlightLegs(Convert.ToInt32(flightDetail.flightID));
            string departedFlightLegID = string.Empty;
            string arrivalFlightLegID = string.Empty;

            if (flightLegList != null && flightLegList.Count > 0)
            {
                departedFlightLegID = flightLegList[0].flightLegID;
                arrivalFlightLegID = flightLegList[flightLegList.Count - 1].flightLegID;
            }
            bool res = adminDataAccess.ModifyFlight(flightDetail.flightID, departedFlightLegID, flightDetail.departTime, arrivalFlightLegID, flightDetail.arrivalTime);
            if (res)
            {
                return Json(new { returnCode = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { returnCode = -1 }, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult LogOut()
        {
            System.Web.HttpContext.Current.Session.RemoveAll();
            AdminDetail adminDetail = new AdminDetail();
            adminDetail.errorMsg = string.Empty;
            return View("AdminPage", adminDetail);
        }
    }
}
