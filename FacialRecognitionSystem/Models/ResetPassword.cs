using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage ="New Password Required.", AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword" , ErrorMessage ="password and confirm password not match !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}