using Microsoft.AspNetCore.Http;
using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class ApplicationUserViewModel
    {
        public Guid UserId{ get; set; }

        [Display(ResourceType = typeof(Labels), Name = "UserName")]
        [RegularExpression(CommonHelper.RegexUsername, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "UserNameRequired", ErrorMessageResourceType = typeof(Messages))]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Position")]
        [Required(ErrorMessageResourceName = "PositionRequired", ErrorMessageResourceType = typeof(Messages))]
        public string position { get; set; }
        
        [Display(ResourceType = typeof(Labels), Name = "skype_id")]
        [Required(ErrorMessageResourceName = "SkypeIdRequired", ErrorMessageResourceType = typeof(Messages))]
        public string skype_id { get; set; }

        [RegularExpression(CommonHelper.RegexEmail, ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [Required(ErrorMessageResourceName = "Passwordrequired", ErrorMessageResourceType = typeof(Messages))]
        public string password { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ConfirmPassword")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [Required(ErrorMessageResourceName = "Passwordrequired", ErrorMessageResourceType = typeof(Messages))]
        public string confirm_password { get; set; }

        //For uploading file
        //   [Required(ErrorMessage = "Required.")]
        public string file { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "File")]
        [Required(ErrorMessageResourceName = "FileRequired", ErrorMessageResourceType = typeof(Messages))]
        public IFormFile FileNew { get; set; }
    }
}
