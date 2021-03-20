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
        [RegularExpression(CommonHelper.RegexText, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
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
        [Required(ErrorMessageResourceName = "Passwordrequired", ErrorMessageResourceType = typeof(Messages))]
        public string password { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ConfirmPassword")]
        [Required(ErrorMessageResourceName = "Passwordrequired", ErrorMessageResourceType = typeof(Messages))]
        public string confirm_password { get; set; }
    }
}
