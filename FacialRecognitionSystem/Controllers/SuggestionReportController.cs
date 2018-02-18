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
    public class SuggestionReportController : Controller
    {
        // GET: SuggestionReport
        public ActionResult celebritySuggestionList(int page = 1, string sort = "firstName", string sortDir = "asc", string search = "")
        {
            MyDbEntities db = new MyDbEntities();
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;

            int skip = (page * pageSize) - pageSize;
            var v = (from a in db.CelebritySuggestions

                     select a


                    );

            var data = v.ToList();
            ViewBag.TotalRows = totalRecord;
            return View(data);
        }
        public List<CelebritySuggestion> suggestions(string search, string sort, string sortdir, int skip, int pageSize, out int totalRecord)
        {

            MyDbEntities db = new MyDbEntities();


            var v = (from a in db.CelebritySuggestions

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

    }
}