using FacialRecognitionSystem.Models;
using DataAccess;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace FacialRecognitionSystem.Controllers.API
{
    public class UserAccountController : ApiController
    {
        [HttpPost]
        [Route("api/UserAccount/Login")]

        //creat a new userLogin.cs file  -done  
        public HttpResponseMessage Login([FromBody]UserLogin userLogin)
        {

            string message = "";
            using (MyDbEntities db = new MyDbEntities())
            {
                UserViewModel model = new UserViewModel();

                var user = db.UserDatas.Where(a => a.Email == userLogin.Email).FirstOrDefault();
                if (user != null)
                {

                    if (string.Compare(Crypto.Hash(userLogin.Password), user.Password) == 0)
                    {
                        var ProfilePic = db.UserPhotoes.Where(c => c.UploaderID == user.UserId).FirstOrDefault();
                        model.UploaderID = user.UserId;
                        model.Link = ProfilePic.Link;

                        return Request.CreateResponse(HttpStatusCode.OK, model);
                    }
                    else
                    {
                        message = "Invalid Password! Try Again";
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                    }

                }
                else
                {

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Password! Try Again");
                }
            }

        }

        [HttpPost]
        [Route("api/UserAccount/Register")]
        public HttpResponseMessage Register([FromBody]UserData user)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    message = "Email is already existed";
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                }
                //password encoding
                user.Password = Crypto.Hash(user.Password);
                user.ActiveStatus = true;

                // add face API recognition here  // using Http MultiPart method -not done
                //------------- implement the code ----------
                // verify a new user by the face
                //save to database

                using (MyDbEntities db = new MyDbEntities())
                {
                    UserViewModel model = new UserViewModel();

                    //var ProfilePic = db.UserPhotos.Where(c => c.UploaderID == user.UserId).FirstOrDefault();
                    
                    //model.Link = ProfilePic.Link;

                    db.UserDatas.Add(user);
                    db.SaveChanges();
                    model.UploaderID = user.UserId;
                    model.Link = "";
                    // map user detail with profile 4to
                    return Request.CreateResponse(HttpStatusCode.OK, model);
                    // create the user profile and load it in the home page of mobile app
                }
            }
            else
            {
                message = "Connection Error! Try Again";
                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }

        }

        [HttpPost]
        [Route("api/UserAccount/UserPhoto")]
        public HttpResponseMessage UserPhoto([FromBody]UserPhoto model)
        {
            if (ModelState.IsValid)
            {

                //save to database
                using (MyDbEntities db = new MyDbEntities())
                {
                    UserViewModel user = new UserViewModel();

                    user.UploaderID = model.UploaderID;
                    user.Link = model.Link;

                    db.UserPhotoes.Add(model);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, model);

                }
            }
            else
            {
                string message = "Photo Upload Failed! Try Again";
                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }
        }

        [HttpPost]
        [Route("api/UserAccount/UserProfile")]
        public HttpResponseMessage UserProfile([FromBody]UserPhoto user)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    UserViewModel model = new UserViewModel();

                    var userProfile = db.UserDatas.Where(a => a.UserId == user.UploaderID).FirstOrDefault();
                    //var userProfile = db.UserPhotos.Where(c => c.UploaderID == user.UserId).FirstOrDefault();

                    //model.Link = ProfilePic.Link;

                    // map user detail with profile 4to
                    return Request.CreateResponse(HttpStatusCode.OK, userProfile);
                    // create the user profile and load it in the home page of mobile app
                }
            }
            else
            {
                message = "Connection Error! Try Again";
                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }

        }

        [NonAction]
        public Boolean IsEmailExist(string email)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var existState = db.UserDatas.Where(a => a.Email == email).FirstOrDefault();
                return existState != null;
            }
        }


        [NonAction]
        public void SendVerificationLinkEmail(string email, string activatonCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activatonCode;
            var link = ConfigurationManager.AppSettings["Host"].ToString() + verifyUrl;
            //Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("thrinduchulle@gmail.com", "thrindu chulle");//your email address
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "****";//email password

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
    }
}
