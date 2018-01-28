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
using PagedList;
using FaceAPIFunctions;

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

        public ActionResult GetCelebrity(int page = 1,int pageSize = 10)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                List<Celebrity> celebrity = db.Celebrities.ToList();
                if (celebrity != null) {
                    PagedList<Celebrity> model = new PagedList<Celebrity>(celebrity, page, pageSize);
                    return View(model);
                }
                return View();
            }
        }
        
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
            Face0 faceAPI = new Face0();
            HttpPostedFileBase file = celebrity.imageBrowes;
            //
            Image i = Image.FromStream(file.InputStream, true, true);
            MemoryStream ms = new MemoryStream();
            i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgData = ms.ToArray();

            using (var fileStream = new MemoryStream(imgData))
            {
                
                int s = await  faceAPI.searchFirst(fileStream);
                if (s != 3)
                {
                    ViewBag.Status = true;
                    ViewBag.Message = "error";
                    return View();
                }
            }
            //
            try
            {
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
                    //
                    
                    using (var fileStream = new MemoryStream(imgData))
                    {
                        string s = await faceAPI.register(imgData, id);
                        if(s != "Success")
                        {
                            ViewBag.Status = true;
                            ViewBag.Message = "error";
                            return View();
                        }
                    }
                    //
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
                        //Image i = Image.FromStream(file.InputStream, true, true);
                        //MemoryStream ms = new MemoryStream();
                        //i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //byte[] imgData = ms.ToArray();

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
                                ViewBag.Status = true;
                                ViewBag.Message = "Error occured while creating profile";
                                return View(celebrity);
                            }
                        }
                        else
                        {
                            ViewBag.Status = true;
                            ViewBag.Message = "Error occured while creating profile";
                            return View(celebrity);
                        }
                            
                    }
                    else
                    {
                        ViewBag.Status = true;
                        ViewBag.Message = "Error occured while creating profile";
                        return View(celebrity);
                    }
                }


                return View();
            }
            catch(Exception e)
            {
                ViewBag.Status = true;
                ViewBag.Message = "Error occured while creating profile";
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
        public async Task<ActionResult> Search(HttpPostedFileBase imageBrowes)
        {
            Image i = Image.FromStream(imageBrowes.InputStream, true, true);
            MemoryStream ms = new MemoryStream();
            i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgData = ms.ToArray();

            Face0 faceAPI = new Face0();
            using (var fileStream = new MemoryStream(imgData))
            {
                int count = 0;
                int[] message = await faceAPI.search(fileStream);
                for(int j = 0; j < 10; j++)
                {
                    if(message[j] == -1)
                    {
                        ViewBag.Message = "Not Faces in Image";
                        return View();
                    }else if(message[j] != 0)
                    {
                        count++;
                    }
                }
                if(count == 0)
                {
                    //no one identify
                    return View();
                }
                else
                {
                    using(MyDbEntities db = new MyDbEntities())
                    {
                        var x = db.Celebrities.Where(a => a.CelebrityId == message[0] || a.CelebrityId == message[1] || a.CelebrityId == message[2] || a.CelebrityId == message[3] || a.CelebrityId == message[4]).ToList();
                    }
                    return View();
                }
                /*if(message != "no face detected" && message != "No one identified")
                {
                    foreach(char str in message)
                    {

                    }
                }
                else
                {
                    ViewBag.Status = true;
                    ViewBag.Message = message;
                    return View();
                }*/
            }


        }

        [HttpPost]
        public ActionResult NameSearch(Celebrity model)
        {
            using (MyDbEntities db = new MyDbEntities()) {
                var celebritySet = db.Celebrities.Where(a => a.FirstName == model.FirstName || a.LastName ==model.FirstName).ToList();
                
                if (celebritySet.Count != 0)
                {
                    return View(celebritySet);
                    
                }
                return View();
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
            return RedirectToAction("Search", "Celebrity");
        }

        [HttpPost]
        public ActionResult Changesetting(CelebrityViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    var celebrity = db.Celebrities.Where(a => a.CelebrityId == model.CelebrityId).FirstOrDefault();
                    if (celebrity != null)
                    {
                        celebrity.FirstName = model.FirstName;
                        celebrity.LastName = model.LastName;
                        celebrity.Gender = model.Gender;
                        celebrity.Feild = model.Feild;
                        celebrity.Description = model.Description;
                        celebrity.ActiveStatus = (model.ActiveStatus);
                        db.SaveChanges();
                        return RedirectToAction("CelebrityProfile", "Celebrity", new {id= celebrity.CelebrityId });
                    }
                    ViewBag.Status = true;
                    ViewBag.Message = "Not Updated.";
                    return View(model);
                }
                
            }
            ViewBag.Status = true;
            ViewBag.Message = "Not Updated.";
            return View(model);
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
                        return RedirectToAction("CelebrityProfile", new { id = model.CelebrityId });
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
        
        public ActionResult SetProfilePic(int id ,int pId)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var photos = db.CelebrityPhotoes.Where(a => a.CelibrityID == id).ToList();
                foreach(var img in photos)
                {
                    if(img.PhotoID != pId)
                    {
                        img.ProfilePic = false;
                    }
                    else
                    {
                        img.ProfilePic = true;
                    }
                    
                }
                db.SaveChanges();
                return RedirectToAction("CelebrityProfile", new { id = id });
            }
        }

    }
}