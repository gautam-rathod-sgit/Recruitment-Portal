using Microsoft.AspNetCore.Http;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace RecruitmentPortal.WebApp.ViewModels
{
    public class CandidateViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [DisplayName("Applyied Date")]
        public DateTime apply_date { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "CandidateName")]
        [RegularExpression(CommonHelper.RegexText, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "CandidateNameRequired", ErrorMessageResourceType = typeof(Messages))]
        public string name { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [RegularExpression(CommonHelper.RegexEmail, ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Messages))]
        public string email { get; set; }

        public bool emailConfirmed { get; set; }
        public string token { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "DateofBirth")]
        [Required(ErrorMessageResourceName = "DateofBirthRequired", ErrorMessageResourceType = typeof(Messages))]
        public DateTime dob { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Phone")]
        [RegularExpression(CommonHelper.RegExTelephone, ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(Messages))]
        public double phone { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Gender")]
        [Required(ErrorMessageResourceName = "GenderRequired", ErrorMessageResourceType = typeof(Messages))]
        public string Gender { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Experience")]
        [Required(ErrorMessageResourceName = "ExperienceRequired", ErrorMessageResourceType = typeof(Messages))]
        public string experience { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "TotalExperience")]
        [Required(ErrorMessageResourceName = "TotalExperienceRequired", ErrorMessageResourceType = typeof(Messages))]
        public string total_experience { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ReleventExperience")]
        [Required(ErrorMessageResourceName = "ReleventExperienceRequired", ErrorMessageResourceType = typeof(Messages))]
        public string relevent_experience { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "CurrentCTC")]
        [Required(ErrorMessageResourceName = "CurrentCTCRequired", ErrorMessageResourceType = typeof(Messages))]
        public double current_ctc { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ExpectedCTC")]
        [Required(ErrorMessageResourceName = "ExpectedCTCRequired", ErrorMessageResourceType = typeof(Messages))]
        public double expected_ctc { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "NoticePeriod")]
        //[RegularExpression(CommonHelper.RegexText, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "NoticePeriodRequired", ErrorMessageResourceType = typeof(Messages))]
        public string notice_period { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "HSC")]
        [RegularExpression(CommonHelper.RegexNumber, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "HSCPercentageRequired", ErrorMessageResourceType = typeof(Messages))]
        public float hsc_perc { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "SSC")]
        [RegularExpression(CommonHelper.RegexNumber, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "SSCPercentageRequired", ErrorMessageResourceType = typeof(Messages))]
        public float ssc_perc { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ApplyingThrough")]
        //[RegularExpression(CommonHelper.RegexText, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "ApplyingThroughRequired", ErrorMessageResourceType = typeof(Messages))]
        public string applying_through { get; set; }


        
        public string degree { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Degree")]
        [Required(ErrorMessageResourceName = "DegreeRequired", ErrorMessageResourceType = typeof(Messages))]
        public int selectedDegree { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "DepartmentName")]
        [Required(ErrorMessageResourceName = "DepartmentRequired", ErrorMessageResourceType = typeof(Messages))]
        public int selectDept { get; set; }




        [Display(ResourceType = typeof(Labels), Name = "Resume")]
        [Required(ErrorMessageResourceName = "ResumeRequired", ErrorMessageResourceType = typeof(Messages))]
        public string resume { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Resume")]
        [Required(ErrorMessageResourceName = "ResumeRequired", ErrorMessageResourceType = typeof(Messages))]
        public IFormFile File { get; set; }


        //for schedules
        public List<SchedulesViewModel> Schedules { get; set; }

        //for carrying jobid to post method
        public int jobpostID { get; set; }

        //for getting the job position
        [Display(ResourceType = typeof(Labels), Name = "JobTitle")]
        public string jobpostName { get; set; }
        //for getting the job role
        public string jobRole { get; set; }

        //is Active candidate
        public bool isActive { get; set; }

        //is Selected candidate
        public bool isSelected { get; set; }

        //is rejected candidate
        public bool isRejected { get; set; }

        //for jobApplicationID
        public int JobAppId { get; set; }

        public string JobStatus { get; set; }

    }
}
