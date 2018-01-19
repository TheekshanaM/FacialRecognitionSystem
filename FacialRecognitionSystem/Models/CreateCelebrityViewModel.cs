using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class CreateCelebrityViewModel
    {
        public int CelebrityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Feild { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase imageBrowes { get; set; }
        public string Link { get; set; }
    }
}