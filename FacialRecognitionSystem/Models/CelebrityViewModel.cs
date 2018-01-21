using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class CelebrityViewModel
    {
        public int CelebrityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Feild { get; set; }
        public string Description { get; set; }
        
        public int Rating { get; set; }

        public string ProfilePic { get; set; }
        public IEnumerable<string> photo { get; set; }

        
    }
}