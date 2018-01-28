using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class LocationViewModel
    {
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }

    }
}