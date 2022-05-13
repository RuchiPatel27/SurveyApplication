using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace SurveyApplication.Models
{
    public class RegistrationViewModel
    {
        [Required]
        [Display(Name = "FirstName: ")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "MiddleName: ")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "LastName: ")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Address: ")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City: ")]
        public string City { get; set; }

        [Required]
        [Display(Name = "DateofBirth: ")]
        public DateTime DateofBirth { get; set; }
        
        [Required(ErrorMessage ="This field is required")]
        public string IsSurveyor { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address: ")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}