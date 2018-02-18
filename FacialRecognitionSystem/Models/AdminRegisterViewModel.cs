using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacialRecognitionSystem.Models
{
    public class AdminRegisterViewModel
    {
        [Display(Name = "First Name ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is Required.")]
        [RegularExpression(@"[a-zA-Z]*",ErrorMessage = "only use alphabet")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required.")]
        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "only use alphabet")]
        public string LastName { get; set; }

        [Display(Name = "Email ")]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required.")]
        public string Email { get; set; }

        [Display(Name = "Password ")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,15}",ErrorMessage = "Minimum 6 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password ")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password not match.")]
        public string ConfirmPassword { get; set; }


    }
}