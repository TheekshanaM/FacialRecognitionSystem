
using FacialRecognitionSystem.Models;

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
    }
}
