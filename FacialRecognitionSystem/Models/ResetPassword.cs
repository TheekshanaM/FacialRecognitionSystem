using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class ResetPassword
    {
        [Display(Name = "New Password ")]
        [Required(ErrorMessage ="New Password Required.", AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,15}", ErrorMessage = "Minimum 6 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password ")]
        [Compare("NewPassword" , ErrorMessage ="password and confirm password not match !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}