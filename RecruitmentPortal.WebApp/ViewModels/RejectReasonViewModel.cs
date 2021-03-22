using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class RejectReasonViewModel
    {
        //for CandidateID
        public string CandidateId { get; set; }
        //for jobApplicationID
        public int JobAppId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "RejectionReason")]
        [RegularExpression(CommonHelper.RegexText, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "RejectionReasonRequired", ErrorMessageResourceType = typeof(Messages))]
        public string rejection_reason { get; set; }
    }
}
