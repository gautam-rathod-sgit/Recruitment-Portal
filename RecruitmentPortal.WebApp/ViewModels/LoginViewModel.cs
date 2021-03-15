using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class LoginViewModel
    {
        [RegularExpression(CommonHelper.RegexEmail, ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "Emailrequired", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Passwordrequired", ErrorMessageResourceType = typeof(Messages))]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
