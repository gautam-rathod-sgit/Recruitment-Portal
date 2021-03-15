using Microsoft.AspNetCore.Http;
using RecruitmentPortal.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class RegisterViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string position { get; set; }
        public string skype_id { get; set; }


        //For uploading file
        //   [Required(ErrorMessage = "Required.")]
        public string file { get; set; }
        public IFormFile FileNew { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
