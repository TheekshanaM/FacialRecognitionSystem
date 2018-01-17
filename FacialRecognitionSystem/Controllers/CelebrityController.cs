using FacialRecognitionSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FacialRecognitionSystem.Controllers
{
    public class CelebrityController : Controller
    {
        HttpClient client = new HttpClient();

        

        public CelebrityController()
        {
            client.BaseAddress = new Uri("http://localhost:13138/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET: Celebrity
        [HttpGet]
        public ActionResult NewCelebrity()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<ActionResult> NewCelebrity(Celebrity celebrity)
        {
            int message;
            if (ModelState.IsValid)
            {
                celebrity.Gender = celebrity.Gender.ToString();
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(celebrity);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/Celebrity/NewCelebrity", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<int>().Result;
                    
                     
                    if (message != 0)
                    {
                        
                        return RedirectToAction("UploadPhoto", new { id = message });
                    }
                    else
                    {
                        return View(celebrity);
                    }
                }
            }
            else
            {
                return View(celebrity);
            }
            return View(celebrity);
        }

        
        public ActionResult UploadPhoto(int id)
        {
            PhotoUploadModel model = new PhotoUploadModel();
            model.id = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadPhoto(PhotoUploadModel model)
        {
            HttpPostedFileBase file = model.imageBrowes;
            try
            {
                string _path = "";
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    _path = Path.Combine(Server.MapPath("~/Photo/Celebrity"), _FileName);
                    file.SaveAs(_path);
                }
                
                CelebrityPhoto cmodel = new CelebrityPhoto();
                cmodel.CelibrityID = model.id;
                cmodel.Link = "/Photo/Celebrity/" + file.FileName;
                using(MyDbEntities db = new MyDbEntities())
                {
                    db.CelebrityPhotoes.Add(cmodel);
                    db.SaveChanges();
                }
                return RedirectToAction("CelebrityProfile");
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return null;
            }
        }

        public ActionResult CelebrityProfile()
        {
            return View();
        }
        
    }
}