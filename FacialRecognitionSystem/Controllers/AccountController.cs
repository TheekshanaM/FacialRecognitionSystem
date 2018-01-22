using FacialRecognitionSystem.Models;
using DataAccess;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;


namespace FacialRecognitionSystem.Controllers
{
    public class AccountController : Controller
    {
        HttpClient client = new HttpClient();

        public AccountController()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Host"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Exclude = "isEmailVerified , activationCode")] AdminRegisterViewModel admin)
        {
            Boolean status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(admin);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/AdminAccount/Register", stringContent);
                ViewBag.Message = response.ToString();
                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                    if(message == "EmailExist")
                    {
                        ModelState.AddModelError("EmailExist", "Email is already exist !");
                        return View(admin);
                    }else if(message == "For activation check your email.")
                    {
                        status = true;
                    }
                }
            }
            else
            {
                message = "Invalid Request !";
            }
            
            ViewBag.Status = status;

            return View(admin);
        }

        [HttpGet]
        public async Task<ActionResult> VerifyAccount(string id)
        {
            Boolean status = false;
            if(id != null)
            {
                HttpResponseMessage response = await client.GetAsync("API/AdminAccount?id="+id);
                if (response.IsSuccessStatusCode)
                {
                    status = response.Content.ReadAsAsync<Boolean>().Result;
                    if (status == false)
                    {
                        ViewBag.Message = "Invalid Request !";
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }

            }
            else
            {
                ViewBag.Message = "Invalid Request !";
            }
            
            ViewBag.Status = status;
            return View();

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AdminLogin adminLogin, string returnUrl = "")
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(adminLogin);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("API/AdminAccount/Login", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                    if (message == "Success")
                    {
                        int timeOut = 0;
                        if (adminLogin.RememberMe)
                        {
                            timeOut = 525600;
                        }
                        else
                        {
                            timeOut = 20;
                        }
                        var ticket = new FormsAuthenticationTicket(adminLogin.Email, adminLogin.RememberMe, timeOut);
                        string encripted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encripted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeOut);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                }
                else
                {
                    message = "Invalid Request";
                }
            }
            else
            {
                message = "Invalid Request";
            }
            
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        public ActionResult LogOut()
        {
            
            

            FormsAuthentication.SignOut();
            

            return RedirectToAction("Login", "Account");
        }

        

        [HttpGet]
        public ActionResult FogetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FogetPassword(string email)
        {
            string message = "";
            if(email != null)
            {
                HttpResponseMessage response = await client.GetAsync("API/AdminAccount?Email=" + email);
                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                }
            }
            else
            {
                message = "Enter Email";
            }
            

            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ResetPassword(string id)
        {
            
            if (id != null)
            {
                HttpResponseMessage response = await client.GetAsync("API/AdminAccount?ResetPasswordCode=" + id);
                if (response.IsSuccessStatusCode)
                {
                    Admin singleUser = response.Content.ReadAsAsync<Admin>().Result;
                    if (singleUser != null)
                    {
                        ResetPassword resetModle = new ResetPassword();
                        resetModle.ResetCode = id;
                        ViewBag.Status = false;
                        return View(resetModle);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return HttpNotFound();
                }

            }
            else
            {
                return HttpNotFound();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassword model)
        {
            var message = "";
            Boolean status = false;
            if (ModelState.IsValid)
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(model);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("API/AdminAccount/ResetPassword", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    message = response.Content.ReadAsAsync<string>().Result;
                    if (message == "New password Updated Successfully")
                    {
                        status = true;
                    }
                }
                else
                {
                    message = "invalid !";
                }
            }
            else
            {
                message = "invalid !";
            }
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeSetting()
        {
            Admin admin = new Admin();
            admin.Email = System.Web.HttpContext.Current.User.Identity.Name;

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(admin);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/AdminAccount/ViewDetails", stringContent);
            if (response.IsSuccessStatusCode)
            {
                admin = response.Content.ReadAsAsync<Admin>().Result;
                return View(admin);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSetting(Admin admin)
        {
            admin.Email = System.Web.HttpContext.Current.User.Identity.Name;
            using (MyDbEntities db = new MyDbEntities())
            {
                Admin account = db.Admins.Where(a => a.Email == admin.Email).FirstOrDefault();
                if(account != null)
                {
                    account.FirstName = admin.FirstName;
                    account.LastName = admin.LastName;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(admin);
                }
            }
        }
    }
}