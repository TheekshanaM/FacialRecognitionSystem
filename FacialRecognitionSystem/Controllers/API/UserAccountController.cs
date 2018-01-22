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
        public UserData Login([FromBody]UserLogin userLogin)
        {

            string message = "";
            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.UserDatas.Where(a => a.Email == userLogin.Email).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(userLogin.Password), user.Password) == 0)
                    {
                        message = "Success";
                        return user;
                    }
                    else
                    {
                        message = "Invalid credential Provided";
                        return null;
                    }

                }
                else
                {
                    message = "Invalid credential Provided";
                    return null;
                }
            }
            
        }

        [HttpPost]
        [Route("api/UserAccount/Register")]
        public UserData Register([FromBody]UserData user)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    return null;
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
                    db.UserDatas.Add(user);
                    db.SaveChanges();
                    // map user detail with profile 4to
                    return user;
                    // create the user profile and load it in the home page of mobile app
                }
            }
            else
            {
                return null;
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
