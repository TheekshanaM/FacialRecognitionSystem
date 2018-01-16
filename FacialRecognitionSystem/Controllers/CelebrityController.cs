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
            string message = "";
            if (ModelState.IsValid)
            {
                celebrity.Gender = celebrity.Gender.ToString();
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(celebrity);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/Celebrity/NewCelebrity", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                    if (message == "Success")
                    {

                        return View();
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

        
    }
}