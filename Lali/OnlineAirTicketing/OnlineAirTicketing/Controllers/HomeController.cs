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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ValidateUser(UserDetail userDetail)
        {
            UserDataAccess userDataAccess = new UserDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            int resultUserID = userDataAccess.validateUserInfo(userDetail.username, userDetail.password);
            UserDetail userTempDetail = userDataAccess.getUserInfo(resultUserID);
            if (resultUserID > 0)
            {
                System.Web.HttpContext.Current.Session["userID"] = userDetail.userID;
                System.Web.HttpContext.Current.Session["name"] = userDetail.personName;
                return View("UserDashboard", userTempDetail);
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
            UserDataAccess userDataAccess = new UserDataAccess(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            int userID = userDataAccess.addUser(userDetail);
            //UserDetail userDetail = userDataAccess.getUserInfo(userID);
            if (userID > 0)
            {
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
        
    }
}
