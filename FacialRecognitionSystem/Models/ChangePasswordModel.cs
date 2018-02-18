using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class ChangePasswordModel
    {
        [Key]
        public int AdminId { get; set; }

        [Display(Name = "Current password")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "New password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,15}", ErrorMessage = "Minimum 6 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Password mismatch")]
        public string ConfirmPassword { get; set; }
    }
}