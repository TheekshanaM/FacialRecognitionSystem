using FacialRecognitionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacialRecognitionSystem.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "isEmailVerified , activationCode")] Admin admin)
        {
            Boolean status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = IsEmailExist(admin.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email is already exist !");
                    return View(admin);
                }

                //Generate Activation code
                admin.ActivationCode = Guid.NewGuid();

                //password encoding
                admin.Password = Crypto.Hash(admin.Password);
                admin.ConfirmPassword = Crypto.Hash(admin.ConfirmPassword);

                admin.IsEmailVerified = false;

                //save to database
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.Admins.Add(admin);
                    db.SaveChanges();

                    //send Email
                    SendVerificationLinkEmail(admin.Email, admin.ActivationCode.ToString());
                    message = "For activation check your email.";
                    status = true;
                }
            }
            else
            {
                message = "Invalid Request !";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;

            return View(admin);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            Boolean status = false;
            using (MyDbEntities db = new MyDbEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var validActivation = db.Admins.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

                if (validActivation != null)
                {
                    validActivation.IsEmailVerified = true;
                    db.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request !";
                }
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
        public ActionResult Login(AdminLogin adminLogin, string returnUrl = "")
        {
            string message = "";
            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.Admins.Where(a => a.Email == adminLogin.Email).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(adminLogin.Password), user.Password) == 0)
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
                    else
                    {
                        message = "Invalid credential Provided";
                    }

                }
                else
                {
                    message = "Invalid credential Provided";
                }
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

        [NonAction]
        public Boolean IsEmailExist(string email)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var existState = db.Admins.Where(a => a.Email == email).FirstOrDefault();
                return existState != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activatonCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activatonCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("thrinduchulle@gmail.com", "thrindu chulle");//your email address
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "inspiron15";//your email password

            string subject = "";
            string body = "";

            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successdully created !";
                body = "<br/><br/>Click <a href='" + link + "'>here<a/>";
            }
            else
            {
                subject = "Reset password";
                body = "Click <a href='" + link + "'>Here<a/> to Reset password.";
            }



            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                try
                {
                    smtp.Send(message);

                }
                catch (Exception e)
                {

                }

        }

        [HttpGet]
        public ActionResult FogetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FogetPassword(string email)
        {
            string message = "";
            Boolean status = false;

            using (MyDbEntities db = new MyDbEntities())
            {
                var singlUeser = db.Admins.Where(a => a.Email == email).FirstOrDefault();
                if (singlUeser != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(singlUeser.Email, resetCode, "ResetPassword");
                    singlUeser.ResetPasswordCode = resetCode;

                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.SaveChanges();
                    message = "Reset password link has been sent your Email.";
                }
                else
                {
                    message = "Account Not found !";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var singleUser = db.Admins.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (singleUser != null)
                {
                    ResetPassword resetModle = new ResetPassword();
                    resetModle.ResetCode = id;
                    return View(resetModle);
                }
                else
                {
                    return HttpNotFound();
                }
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    var singelUser = db.Admins.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (singelUser != null)
                    {
                        singelUser.Password = Crypto.Hash(model.NewPassword);
                        singelUser.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New password Updated Successfully";
                    }

                }
            }
            else
            {
                message = "invalid !";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}