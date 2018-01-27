using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacialRecognitionSystem.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NameSearch(UserData model)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var UserSet = db.UserDatas.Where(a => a.FirstName == model.FirstName || a.LastName == model.FirstName).ToList();

                if (UserSet.Count != 0)
                {
                    return View(UserSet);

                }
                return View();
            }
        }
    }
}