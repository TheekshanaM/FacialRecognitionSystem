using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class CreateCelebrityViewModel
    {

        public int CelebrityId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name Required !")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name Required !")]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender Required !")]
        public string Gender { get; set; }
        public string Feild { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase imageBrowes { get; set; }
        public string Link { get; set; }
    }
}