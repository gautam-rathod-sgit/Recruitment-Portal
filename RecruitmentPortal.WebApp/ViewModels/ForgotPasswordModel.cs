using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class ForgotPasswordModel
    {
        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [RegularExpression(CommonHelper.RegexEmail, ErrorMessageResourceName = "ValidEmailRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }
    }
}
