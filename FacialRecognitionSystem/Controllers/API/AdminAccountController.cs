﻿using FacialRecognitionSystem.Models;
using DataAccess;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace FacialRecognitionSystem.Controllers.API
{
    public class AdminAccountController : ApiController
    {


        [HttpPost]
        [Route("api/AdminAccount/Login")]

        public string Login([FromBody]AdminLogin adminLogin)
        {

            string message = "";
            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.Admins.Where(a => a.Email == adminLogin.Email && a.IsEmailVerified == true).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(adminLogin.Password), user.Password) == 0)
                    {
                        if(user.IsEmailVerified == true)
                        {
                            message = "Success";
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
                else
                {
                    message = "Invalid credential Provided";
                }
            }
            return message;
        }

        [HttpPost]
        [Route("api/AdminAccount/Register")]
        public string Register([FromBody]AdminRegisterViewModel adminModel)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = IsEmailExist(adminModel.Email);
                if (isExist)
                {
                    return "EmailExist";
                }
                //password encoding
                adminModel.Password = Crypto.Hash(adminModel.Password);
                adminModel.ConfirmPassword = Crypto.Hash(adminModel.ConfirmPassword);
                Admin admin = new Admin();
                admin.FirstName = adminModel.FirstName;
                admin.LastName = adminModel.LastName;
                admin.Email = adminModel.Email;
                admin.Password = adminModel.Password;
                admin.IsEmailVerified = false;
                //Generate Activation code
                admin.ActivationCode = Guid.NewGuid();
                

                //save to database
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.Admins.Add(admin);
                    db.SaveChanges();

                    //send Email
                    SendVerificationLinkEmail(adminModel.Email, admin.ActivationCode.ToString());
                    message = "For activation check your email.";

                    return message;
                }
            }
            else
            {
                return "Invalid Request";
            }

        }

        [HttpGet]
        public Boolean VerifyAccount(string id)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var validActivation = db.Admins.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

                if (validActivation != null)
                {
                    validActivation.IsEmailVerified = true;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [HttpGet]
        public string FogetPassword(string email)
        {
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
                    return "Reset password link has been sent your Email.";
                }
                else
                {
                    return "Account Not found !";
                }
            }
        }

        [HttpGet]
        public Admin ResetPassword(string ResetPasswordCode)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                return db.Admins.Where(a => a.ResetPasswordCode == ResetPasswordCode).FirstOrDefault();
            }
        }

        [HttpPost]
        [Route("api/AdminAccount/ResetPassword")]
        public string ResetPassword(ResetPassword model)
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
                    return "New password Updated Successfully";
                }
                return "";
            }
        }

        [HttpPost]
        [Route("api/AdminAccount/ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                using(MyDbEntities db = new MyDbEntities())
                {
                    var admin = db.Admins.Where(a => a.AdminId == model.AdminId).FirstOrDefault();
                    if(admin != null)
                    {
                        if (string.Compare(Crypto.Hash(model.Password), admin.Password) == 0)
                        {
                            admin.Password = Crypto.Hash(model.NewPassword);
                            db.SaveChanges();
                            return Request.CreateResponse(HttpStatusCode.OK, "Succes");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Current password is wrong");
                        }

                    }
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "invalid.");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "invalid.");
            }
        }

        [NonAction]
        public Boolean IsEmailExist(string email)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var existState = db.Admins.FirstOrDefault(a => a.Email == email);
                    //db.Admins.Where(a => a.Email == email).FirstOrDefault();
                return existState != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activatonCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activatonCode;
            var link = ConfigurationManager.AppSettings["Host"].ToString() + verifyUrl;
                //Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString(), "uomzircon tech");//your email address
            var toEmail = new MailAddress(email);
            var fromEmailPassword = ConfigurationManager.AppSettings["Password"].ToString();//email password

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

        [HttpPost]
        [Route("api/AdminAccount/ViewDetails")]
        public Admin ViewDetails([FromBody]Admin admin)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var adminAccount = db.Admins.Where(a => a.Email == admin.Email).FirstOrDefault();
                Admin account = new Admin();
                account.AdminId = adminAccount.AdminId;
                account.FirstName = adminAccount.FirstName;
                account.LastName = adminAccount.LastName;
                
                return account;
            }
        }

    }
}
