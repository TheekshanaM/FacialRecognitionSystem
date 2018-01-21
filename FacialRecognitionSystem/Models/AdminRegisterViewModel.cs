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
        public string FirstName { get; set; }

        [Display(Name = "Last Name ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }

        [Display(Name = "Email ")]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required.")]
        public string Email { get; set; }

        [Display(Name = "Password ")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required.")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters Required.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password ")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password not match.")]
        public string ConfirmPassword { get; set; }


    }
}