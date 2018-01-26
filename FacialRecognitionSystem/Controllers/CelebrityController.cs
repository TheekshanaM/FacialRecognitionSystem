using FacialRecognitionSystem.Models;
using DataAccess;

using System;
using System.Collections.Generic;
using System.Configuration;
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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;

namespace FacialRecognitionSystem.Controllers
{
    [Authorize]
    public class CelebrityController : Controller
    {
        HttpClient client = new HttpClient();

        

        public CelebrityController()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Host"].ToString());
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
                
                

                
                celebrity.Link = "/Photo/Celebrity/" + file.FileName;
                Celebrity celebrityModel = new Celebrity();
                celebrityModel.FirstName = celebrity.FirstName;
                celebrityModel.LastName = celebrity.LastName;
                celebrityModel.Gender = celebrity.Gender;
                celebrityModel.Feild = celebrity.Feild;
                celebrityModel.Description = celebrity.Description;
                celebrityModel.ActiveStatus = true;

                

                var serializer = new JavaScriptSerializer();
                var json1 = serializer.Serialize(celebrityModel);
                
                var stringContent1 = new StringContent(json1, Encoding.UTF8, "application/json");
                

                HttpResponseMessage response = await client.PostAsync("api/Celebrity/NewCelebrity", stringContent1);

                if (response.IsSuccessStatusCode)
                {
                    id = response.Content.ReadAsAsync<int>().Result;

                    DateTime dTime = DateTime.Now;
                    string time = dTime.ToString();
                    time = time.Replace(" ", "_")+ ".jpg";

                    if (file.ContentLength > 0)
                    {
                        
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=faceitphotos;AccountKey=67nq3VNJlZ0KJArJZU62vjri4pNzqd1MERWFQytw7w7B6cfTv7Gw75iJq4LJgUN7E05Y0+3ixmkOWDyKk4yhtw==");
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference("celebrityimages");
                        container.CreateIfNotExists();

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(id.ToString()+"_"+time);
                        Image i = Image.FromStream(file.InputStream, true, true);
                        MemoryStream ms = new MemoryStream();
                        i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] imgData = ms.ToArray();

                        using (var fileStream = new MemoryStream(imgData))
                        {
                            await blockBlob.UploadFromStreamAsync(fileStream);
                        }

                    }
                    if (id != 0)
                    {
                        CelebrityPhoto photoModel = new CelebrityPhoto();
                        photoModel.Link = "https://faceitphotos.blob.core.windows.net/celebrityimages/"+id+"_"+time;
                        photoModel.ProfilePic = true;
                        photoModel.CelibrityID = id;
                        var json2 = serializer.Serialize(photoModel);
                        var stringContent2 = new StringContent(json2, Encoding.UTF8, "application/json");
                        response = await client.PostAsync("api/Celebrity/CelebrityPhoto", stringContent2);

                        if (response.IsSuccessStatusCode)
                        {
                            message = response.Content.ReadAsAsync<string>().Result;
                            if(message == "Success")
                            {
                                
                                return RedirectToAction("CelebrityProfile", new {id=id });
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

        public async Task<ActionResult> CelebrityProfile(int id)
        {
            HttpResponseMessage response = await client.GetAsync("API/Celebrity?id=" + id);
            if (response.IsSuccessStatusCode)
            {
                CelebrityViewModel model = response.Content.ReadAsAsync<CelebrityViewModel>().Result;
                return View(model);
            }

            
            return RedirectToAction("NewCelebrity");
        }
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NameSearch(Celebrity model)
        {
            using (MyDbEntities db = new MyDbEntities()) {
                var celebritySet = db.Celebrities.Where(a => a.FirstName == model.FirstName).ToList();
                if(celebritySet != null)
                {
                    return View(celebritySet);
                    
                }
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> Changesetting(int id)
        {
            HttpResponseMessage response = await client.GetAsync("API/Celebrity?id=" + id);
            if (response.IsSuccessStatusCode)
            {
                CelebrityViewModel model = response.Content.ReadAsAsync<CelebrityViewModel>().Result;
                return View(model);
            }
            return RedirectToAction("NewCelebrity");
        }

        [HttpPost]
        public ActionResult Changesetting(CelebrityViewModel model)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var celebrity = db.Celebrities.Where(a => a.CelebrityId == model.CelebrityId).FirstOrDefault();
                celebrity.FirstName = model.FirstName;
                celebrity.LastName = model.LastName;
                celebrity.Gender = model.Gender;
                celebrity.Feild = model.Feild;
                celebrity.Description = model.Description;
                celebrity.ActiveStatus = (model.ActiveStatus);
                db.SaveChanges();
                return RedirectToAction("Changesetting", "Celebrity",new { celebrity.CelebrityId});
            }
        }

        public ActionResult ViewImage(int id)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var imageList = db.CelebrityPhotoes.Where(a => a.CelibrityID == id).ToList();
                if(imageList != null)
                {
                    ViewBag.Message = id;
                    return View(imageList);
                }
                return RedirectToAction("Changesetting", "Celebrity", new { id});
            }
        }
        
        public async Task<ActionResult> AddPhoto(CreateCelebrityViewModel model)
        {
            string message = "";
            HttpPostedFileBase file = model.imageBrowes;

            DateTime dTime = DateTime.Now;
            string time = dTime.ToString();
            time = time.Replace(" ", "_") + ".jpg";

            if (file.ContentLength > 0)
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=faceitphotos;AccountKey=67nq3VNJlZ0KJArJZU62vjri4pNzqd1MERWFQytw7w7B6cfTv7Gw75iJq4LJgUN7E05Y0+3ixmkOWDyKk4yhtw==");
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("celebrityimages");
                container.CreateIfNotExists();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(model.CelebrityId.ToString() + "_" + time);
                Image i = Image.FromStream(file.InputStream, true, true);
                MemoryStream ms = new MemoryStream();
                i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imgData = ms.ToArray();

                using (var fileStream = new MemoryStream(imgData))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }

                using(MyDbEntities db = new MyDbEntities())
                {
                    var photos = db.CelebrityPhotoes.Where(a => a.CelibrityID == model.CelebrityId).ToList();
                    foreach(var photo in photos)
                    {
                        photo.ProfilePic = false;
                        db.SaveChanges();
                    }
                }
                //
                CelebrityPhoto photoModel = new CelebrityPhoto();
                photoModel.Link = "https://faceitphotos.blob.core.windows.net/celebrityimages/" + model.CelebrityId + "_" + time;
                photoModel.ProfilePic = true;
                photoModel.CelibrityID = model.CelebrityId;
                var serializer = new JavaScriptSerializer();
                var json2 = serializer.Serialize(photoModel);
                var stringContent2 = new StringContent(json2, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("api/Celebrity/CelebrityPhoto", stringContent2);

                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                    if (message == "Success")
                    {
                        ViewBag.Message = "Upload Success";
                        return RedirectToAction("Changesetting", new { id = model.CelebrityId });
                    }
                    else
                    {
                        ViewBag.Message = "Upload Failed";
                        return RedirectToAction("ViewImage", new { id = model.CelebrityId });
                    }
                }
                else
                {
                    ViewBag.Message = "Upload Failed";
                    return RedirectToAction("ViewImage", new { id = model.CelebrityId });
                }
                //
            }
            ViewBag.Message = "Upload Failed";
            return RedirectToAction("ViewImage", new { id = model.CelebrityId });
        }
        

    }
}