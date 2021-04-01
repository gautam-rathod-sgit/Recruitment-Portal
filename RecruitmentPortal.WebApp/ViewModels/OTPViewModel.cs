using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class OTPViewModel
    {
        public string email { get; set; }

        [RegularExpression(CommonHelper.RegexNumber,ErrorMessageResourceName = "ValidOTPRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "OTPRequired", ErrorMessageResourceType = typeof(Messages))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public double? otp { get; set; }
        public double token { get; set; }
        public int jobPostId { get; set; }
    }
}
