using DataAccess;
using FaceAPIFunctions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Search(HttpPostedFileBase imageBrowes)
        {
            Image i = Image.FromStream(imageBrowes.InputStream, true, true);
            MemoryStream ms = new MemoryStream();
            i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgData = ms.ToArray();

            Face0 faceAPI = new Face0();
            using (var fileStream = new MemoryStream(imgData))
            {
                int s1, s2, s3, s4, s5, count = 0;
                int[] message = await faceAPI.search(fileStream);

                /*for(int k = 0; k < 5; k++)
                {
                    if (message[k] < 5000)
                    {
                        message[k] = 0;
                    }
                }*/
                s1 = message[0]; s2 = message[1]; s3 = message[2]; s4 = message[3]; s5 = message[4];
                for (int j = 0; j < 5; j++)
                {
                    if (message[j] == -1)
                    {
                        ViewBag.Status = true;
                        ViewBag.Message = "Not Faces in Image";
                        return View();
                    }
                    else if (message[j] != 0)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    //no one identify
                    ViewBag.Status = true;
                    ViewBag.Message = "No one Identified";
                    return View();
                }
                else
                {
                    using (MyDbEntities db = new MyDbEntities())
                    {
                        IEnumerable<UserData> userSet = db.UserDatas.Where(a => a.UserId == s1 || a.UserId == s2 || a.UserId == s3 || a.UserId == s4 || a.UserId == s5).ToList();
                        if (userSet.Count() != 0)
                        {
                            return View("NameSearch", userSet);
                        }
                        else
                        {
                            ViewBag.Status = true;
                            ViewBag.Message = "No one Identified";
                            return View();
                        }

                    }

                }

            }


        }

        [HttpPost]
        public ActionResult NameSearch(UserData model)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                //var UserSet = db.UserDataExtendeds.Where(a => a.FirstName == model.FirstName || a.LastName == model.FirstName).ToList();
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