
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

                //save to database
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
                        var photo = db.CelebrityPhotoes.Where(u => u.CelibrityID == id).Select(u => u.Link).ToList();
                        var ProfilePic = db.CelebrityPhotoes.Where(c => c.CelibrityID == id && c.ProfilePic == true).FirstOrDefault();

                        model.CelebrityId = celebrity.CelebrityId;
                        model.FirstName = celebrity.FirstName;
                        model.LastName = celebrity.LastName;
                        model.Gender = celebrity.Gender;
                        model.Feild = celebrity.Feild;
                        model.Description = celebrity.Description;
                        model.Rating = celebrity.Rating;
                        model.ProfilePic = ProfilePic.Link;
                        model.photo = photo;
                        
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
    }
}
