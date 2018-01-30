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
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, message);
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

                    var userProfile = db.UserDataExtended2.Where(a => a.UserId == user.UploaderID).FirstOrDefault();
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

        [HttpPost]
        [Route("api/UserAccount/Notification")]


        public HttpResponseMessage Notification([FromBody]  UserViewModel userViewModel)
        {



            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.SearchHistories.Where(a => a.SearchedId == userViewModel.UploaderID).ToList();
                if (user != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, user);

                }
                else
                {
                     
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid credential Provided");
                }
            }

        }

        [HttpPost]
        [Route("api/UserAccount/ForgotPassword")]
        //creat a new userLogin.cs file  -done  
        public HttpResponseMessage ForgotPassword([FromBody]UserLogin userLogin)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.UserDatas.Where(a => a.Email == userLogin.Email).FirstOrDefault();
                if (user != null)
                {
                    Random rnd = new Random();
                    int code = rnd.Next(1000,9999 );
                    SendVerificationLinkEmail(user.Email, code);
                    user.ResetCode = code;

                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid credential Provided");
                }
            }

        }

        [HttpPost]
        [Route("api/UserAccount/Code")]
        public HttpResponseMessage Code ([FromBody]ForgotPasswordModel model)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var user = db.UserDatas.Where(a => a.Email == model.Email).FirstOrDefault();
                if(user != null)
                {
                    if(user.ResetCode == model.ResetCode )
                    {
                        user.Password = model.Password;
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Credential Provided");
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Credential Provided");
                }
            }
        }

        [HttpPost]
        [Route("api/UserAccount/EditProfile")]
        public HttpResponseMessage EditProfile([FromBody]UserData user)
        {
            string message = "";
            UserViewModel model = new UserViewModel();
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    
                    var editProfile = db.UserDatas.Where(a => a.UserId == user.UserId).FirstOrDefault();

                    if (editProfile != null)
                    {
                        editProfile.FirstName = user.FirstName;
                        editProfile.LastName = user.LastName;
                        editProfile.Address = user.Address;
                        editProfile.Gender = user.Gender;
                        editProfile.MobileNumber = user.MobileNumber;
                        editProfile.Description = user.Description;

                        
                        db.SaveChanges();
                        model.UploaderID = user.UserId;
                        
                        return Request.CreateResponse(HttpStatusCode.OK, model);
                        
                    }
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Connection Error! Try Again");
                }
            }
            else
            {
                message = "Connection Error! Try Again";
                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }

        }

        [HttpPost]
        [Route("api/UserAccount/SearchName")]
        public HttpResponseMessage SearchName([FromBody]SearchKeyword search)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    var nameSearch = db.UserDatas.Where(a => a.FirstName == search.Keyword || a.LastName == search.Keyword).ToList();
                    //var userProfile = db.UserPhotos.Where(c => c.UploaderID == user.UserId).FirstOrDefault();

                    //model.Link = ProfilePic.Link;

                    // map user detail with profile 4to
                    return Request.CreateResponse(HttpStatusCode.OK, nameSearch);
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
        [Route("api/UserAccount/SearchFace")]
        public HttpResponseMessage SearchName([FromBody]UserPhoto search)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    var faceSearch = db.UserDatas.Where(a => a.UserId == search.UploaderID).ToList();
                    if (faceSearch != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, faceSearch);
                    }
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Connection Error! Try Again");
                }
            }
            else
            {
                message = "Connection Error! Try Again";
                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }

        }

        [HttpPost]
        [Route("api/UserAccount/CelebrityProfile")]
        public HttpResponseMessage CelebrityProfile([FromBody]CelebrityPhoto celebrity)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    UserViewModel model = new UserViewModel();

                    var celebrProfile = db.Celebrities.Where(a => a.CelebrityId == celebrity.CelibrityID).FirstOrDefault();
                    //var userProfile = db.UserPhotos.Where(c => c.UploaderID == user.UserId).FirstOrDefault();

                    //model.Link = ProfilePic.Link;

                    // map user detail with profile 4to
                    return Request.CreateResponse(HttpStatusCode.OK, celebrProfile);
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
        public void SendVerificationLinkEmail(string email, int ReSetCode)
        {
            
            var fromEmail = new MailAddress("uomzircontech@gmail.com", "thrindu chulle");//your email address
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "FaceItZirconTech";//email password

            string subject = "";
            string body = "";

            
            subject = "Reset password";
            body = "Reset Code : " + ReSetCode; 
            



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
