using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class PhotoUploadModel
    {
        public HttpPostedFileBase imageBrowes { get; set; }
        public int id { get; set; }
    }
}