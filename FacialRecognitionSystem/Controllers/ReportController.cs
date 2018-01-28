using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using System.Linq.Dynamic;
using PagedList;
using FacialRecognitionSystem.Models;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacialRecognitionSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult ReportedList(int page = 1, string sort = "firstName", string sortDir = "asc", string search = "")
        {
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = reportedUsers(search, sort, sortDir, skip, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            return View(data);
        }
        public List<reportedPerson> reportedUsers(string search, string sort, string sortdir, int skip, int pageSize, out int totalRecord)
        {

            MyDbEntities db = new MyDbEntities();


            var v = (from a in db.reportedPersons
                     where a.FirstName.Contains(search) ||
                     a.LastName.Contains(search)
                     select a


                     );

            totalRecord = v.Count();
            v = v.OrderBy(sort + " " + sortdir);
            if (pageSize > 0)
            {
                v = v.Skip(skip).Take(pageSize);

            }

            return v.ToList();


        }
        public ActionResult blockPage(int id)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                ReportedUserModel repU = new ReportedUserModel();
                reportedPerson a = new reportedPerson();

                int x = id;

                var person = db.reportedPersons.Where(u => u.ReportedId == id).FirstOrDefault();
                repU.ReportedUserId = id;
                repU.FirstName = person.FirstName;
                repU.LastName = person.LastName;
                var count = db.reportedPersons.Count(t => t.ReportedId == id);


                var comments =
                                (from recordset in db.reportedPersons
                                 where recordset.ReportedId == id
                                 select recordset.Comment).ToList();


                repU.Comments = comments;
                // foreach(var d in comments)
                repU.TotalReports = count;
                return View(repU);



            }
        }
        public String blockUser(ReportedUserModel e)
        {
            //var repId = id.ReportedUserId;
            using (MyDbEntities db = new MyDbEntities())
            {

                UserData user = db.UserDatas.Single(b => b.UserId == e.ReportedUserId);
                bool a = true;
                user.BlockStatus = a;
                db.SaveChanges();


            }
            return "blocked";
        }
    }
}