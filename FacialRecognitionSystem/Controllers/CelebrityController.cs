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
        
        public async Task<ActionResult> NewCelebrity(CreateCelebrityViewModel celebrity)
        {
            string message ;
            int id;
            HttpPostedFileBase file = celebrity.imageBrowes;
            try
            {
                string _path = "";
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    _path = Path.Combine(Server.MapPath("~/Photo/Celebrity"), _FileName);
                    file.SaveAs(_path);
                }

                
                celebrity.Link = "/Photo/Celebrity/" + file.FileName;
                Celebrity celebrityModel = new Celebrity();
                celebrityModel.FirstName = celebrity.FirstName;
                celebrityModel.LastName = celebrity.LastName;
                celebrityModel.Gender = celebrity.Gender;
                celebrityModel.Feild = celebrity.Feild;
                celebrityModel.Description = celebrity.Description;
                celebrityModel.ActiveStatus = true;

                CelebrityPhoto photoModel = new CelebrityPhoto();
                photoModel.Link = celebrity.Link;

                var serializer = new JavaScriptSerializer();
                var json1 = serializer.Serialize(celebrityModel);
                
                var stringContent1 = new StringContent(json1, Encoding.UTF8, "application/json");
                

                HttpResponseMessage response = await client.PostAsync("api/Celebrity/NewCelebrity", stringContent1);

                if (response.IsSuccessStatusCode)
                {
                    id = response.Content.ReadAsAsync<int>().Result;
                    

                    if (id != 0)
                    {
                        photoModel.CelibrityID = id;
                        var json2 = serializer.Serialize(photoModel);
                        var stringContent2 = new StringContent(json2, Encoding.UTF8, "application/json");
                        response = await client.PostAsync("api/Celebrity/CelebrityPhoto", stringContent2);

                        if (response.IsSuccessStatusCode)
                        {
                            message = response.Content.ReadAsAsync<string>().Result;
                            if(message == "Success")
                            {
                                return RedirectToAction("CelebrityProfile");
                            }
                            else
                            {
                                return View(celebrity);
                            }
                        }
                        else
                        {
                            return View(celebrity);
                        }
                            
                    }
                    else
                    {
                        return View(celebrity);
                    }
                }


                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return null;
            }
            
            
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