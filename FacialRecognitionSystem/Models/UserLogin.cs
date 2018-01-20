using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class UserLogin
    {
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address Required !")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password Required !")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}