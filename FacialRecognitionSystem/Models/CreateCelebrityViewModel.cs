using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class CreateCelebrityViewModel
    {
        [Key]
        public int CelebrityId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name Required !")]
        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "only use alphabet")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name Required !")]
        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "only use alphabet")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender Required !")]
        public string Gender { get; set; }

        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "only use alphabet")]
        public string Feild { get; set; }

        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "only use alphabet")]
        public string Description { get; set; }

        public HttpPostedFileBase imageBrowes { get; set; }
        public string Link { get; set; }
    }
}