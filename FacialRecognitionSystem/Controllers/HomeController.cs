using DataAccess;
using FacialRecognitionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacialRecognitionSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            using (MyDbEntities DB = new MyDbEntities())
            {
                var A = DB.UserDatas;
                var totalUsers = DB.UserDatas.Count();
                var totalAdmins = DB.Admins.Count();
                var totalCelebrities = DB.Celebrities.Count();
                var blocks = DB.UserDatas.Where(t => t.BlockStatus == true).Count();
                FrontPageModel fr = new FrontPageModel();
                fr.tAdmins = totalAdmins;
                fr.tCelebrities = totalCelebrities;
                fr.tUsers = totalUsers;
                fr.blockedUsers = blocks;
                return View(fr);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}