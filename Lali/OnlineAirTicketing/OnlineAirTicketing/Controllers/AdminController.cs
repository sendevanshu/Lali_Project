using DataAccessLayer;
using OnlineAirTicketing.Models;
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
        public ActionResult AdminPage()
        {
            AdminDetail adminDetail = new AdminDetail();
            adminDetail.errorMsg = string.Empty;
            return View(adminDetail);
        }

        public ActionResult ValidateAdmin(AdminDetail adminDetail)
        {
            string viewName = string.Empty;
            bool isLoginSuccessful =  userDataAccess.validateAdminCred(adminDetail.username, adminDetail.password);
            if (!isLoginSuccessful)
            {
                adminDetail.errorMsg = "Invalid Username/Password";
                return View("AdminPage", adminDetail);
            }
            return View("AdminDashboard");
        }

        public JsonResult AddFlight(FlightDetail flightDetail)
        {
            var data = new object();
            return Json(data);

        }
    }
}
