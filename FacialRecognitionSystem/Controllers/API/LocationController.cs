using DataAccess;
using FacialRecognitionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FacialRecognitionSystem.Controllers.API
{
    public class LocationController : ApiController
    {
        [HttpPost]
        [Route("api/Location/SetLocation")]
        public HttpResponseMessage SetLocation([FromBody]Location userLocation)
        {
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    db.Locations.Add(userLocation);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, userLocation);

                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Model Not valied");
        }

        [HttpPost]
        [Route("api/Location/UpdateLocation")]
        public HttpResponseMessage UpdateLocation([FromBody]Location ulocation)
        {


            using (MyDbEntities db = new MyDbEntities())
            {
                var location = db.Locations.Where(a => a.UserID == ulocation.UserID).FirstOrDefault();
                if (location != null)
                {
                    location.Latitude = ulocation.Latitude;
                    location.Longitude = ulocation.Longitude;

                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);

                }
                else
                {
                    
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Location setting failed");
                }
            }


        }

        [HttpPost]
        [Route("api/Location/SearchLocation")]
        public IEnumerable<LocationData> SearchLocation([FromBody]LocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MyDbEntities db = new MyDbEntities())
                {
                    var x = db.LocationDatas.Where(a => a.Latitude > model.MinLatitude && a.Latitude < model.MaxLatitude && a.Longitude > model.MinLongitude && a.Longitude < model.MaxLongitude).ToList();
                    if (x.Count() != 0)
                    {
                        return x;
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
