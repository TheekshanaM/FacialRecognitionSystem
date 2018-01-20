uusing FacialRecognitionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace FacialRecognitionSystem.Controllers.API
{
    public class UserAccountController : ApiController
    {


        [HttpPost]
        [Route("api/UserAccount/Login")]
      
        //creat a new userLogin.cs file  -done  
        public string Login([FromBody]UserLogin userLogin)
        {

            string message = "";
            using (MyDbEntities db = new MyDbEntities())
            {
                var user = db.Users.Where(a => a.Email == userLogin.Email).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(userLogin.Password), user.Password) == 0)
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
            return message;
        }

        [HttpPost]
        [Route("api/UserAccount/Register")]
        public string Register([FromBody]User user)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                //password encoding
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);

                // add face API recognition here  // using Http MultiPart method -not done
                //------------- implement the code ----------
                // verify a new user by the face
                //save to database
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    // map user detail with profile 4to
                    return message = "Success";
                    // create the user profile and load it in the home page of mobile app
                }
            }
            else
            {
                return "Invalid Request";
            }

        }

        [HttpGet]
        public string FogetPassword(string email)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                var singlUeser = db.Users.Where(a => a.Email == email).FirstOrDefault();
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
        public User ResetPassword(string ResetPasswordCode)
        {
            using (MyDbEntities db = new MyDbEntities())
            {
                return db.Users.Where(a => a.ResetPasswordCode == ResetPasswordCode).FirstOrDefault();
            }
        }

        [HttpPost]
        [Route("api/UserAccount/ResetPassword")]
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



        [NonAction]
        public void SendVerificationLinkEmail(string email, string activatonCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activatonCode;
            var link = "http://localhost:13138" + verifyUrl;
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
