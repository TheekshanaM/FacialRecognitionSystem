
using FacialRecognitionSystem.Models;
using DataAccess;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FacialRecognitionSystem.Controllers.API
{
    public class CelebrityController : ApiController
    {
        [HttpPost]
        [Route("api/Celebrity/NewCelebrity")]
        public int NewCelebrity([FromBody]Celebrity celebrity)
        {
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.Celebrities.Add(celebrity);
                    db.SaveChanges();
                    int id = celebrity.CelebrityId;

                    return id;
                }
            }
            else
            {
                return 0;
            }

        }

        [HttpPost]
        [Route("api/Celebrity/CelebrityPhoto")]
        public string CelebrityPhoto([FromBody]CelebrityPhoto model)
        {
            if (ModelState.IsValid)
            {

                //save to database
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.CelebrityPhotoes.Add(model);
                    db.SaveChanges();
                    

                    return "Success";
                }
            }
            else
            {
                return "";
            }
        }

        //[Route("api/Celebrity/ViewCelebrity/{id}")]
        [HttpGet]
        public CelebrityViewModel ViewCelebrity(int id)
        {
            try
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    CelebrityViewModel model = new CelebrityViewModel();
                    var celebrity = db.Celebrities.Where(a => a.CelebrityId == id).FirstOrDefault();
                    if (celebrity != null)
                    {
                        
                        var ProfilePic = db.CelebrityPhotoes.Where(c => c.CelibrityID == id && c.ProfilePic == true).FirstOrDefault();

                        model.CelebrityId = celebrity.CelebrityId;
                        model.FirstName = celebrity.FirstName;
                        model.LastName = celebrity.LastName;
                        model.Gender = celebrity.Gender;
                        model.Feild = celebrity.Feild;
                        model.Description = celebrity.Description;
                        model.ActiveStatus = celebrity.ActiveStatus;
                        model.Rating = celebrity.Rating;
                        model.ProfilePic = ProfilePic.Link;
                        
                        
                        return model;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch(Exception e)
            {
                return null;
            }
            
        }


        [HttpPost]
        [Route("api/Celebrity/SearchName")]
        public HttpResponseMessage SearchName([FromBody]SearchKeyword search)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    var nameSearch = db.Celebrities.Where(a => a.FirstName == search.Keyword || a.LastName == search.Keyword).ToList();
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
        [Route("api/Celebrity/SearchFace")]
        public HttpResponseMessage SearchName([FromBody]CelebrityPhoto search)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    var faceSearch = db.Celebrities.Where(a => a.CelebrityId == search.CelibrityID).ToList();
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
        [Route("api/Celebrity/CelebrityProfile")]
        public HttpResponseMessage CelebrityProfile([FromBody]CelebrityPhoto user)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                using (MyDbEntities db = new MyDbEntities())
                {
                    UserViewModel model = new UserViewModel();

                    var userProfile = db.CelebrityDataExtendeds.Where(a => a.CelebrityId == user.CelibrityID).FirstOrDefault();
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

    }
}
