using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentPortal.Core.Entities;
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
    public class SchedulesViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "CandidateName")]
        [Required(ErrorMessageResourceName = "CandidateNameRequired", ErrorMessageResourceType = typeof(Messages))]
        public string candidate_name { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Position")]
        [Required(ErrorMessageResourceName = "JobTitleRequired", ErrorMessageResourceType = typeof(Messages))]
        public string position { get; set; }

       
        [Display(ResourceType = typeof(Labels), Name = "DateTime")]
        [Required(ErrorMessageResourceName = "DateTimeRequired", ErrorMessageResourceType = typeof(Messages))]
        [DisplayFormat( DataFormatString = "{dd-MM-yyyy hh:mm A}")]
        //  [RegularExpression(CommonHelper.DateFormat, ErrorMessageResourceName = "DateFormat", ErrorMessageResourceType = typeof(Messages))]
        public DateTime datetime { get; set; }
        public string FormattedDate { get; set; }
        public string time { get; set; }

        //enum round
        [EnumDataType(typeof(RoundType))]
        [Display(ResourceType = typeof(Labels), Name = "Round")]
        [Required(ErrorMessageResourceName = "RoundRequired", ErrorMessageResourceType = typeof(Messages))]
        public RoundType roundValue { get; set; }
        public int round { get; set; }

        
        public string roundName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Interviewer")]
        [Required(ErrorMessageResourceName = "InterviewerRequired", ErrorMessageResourceType = typeof(Messages))]
        public List<string> Multiinterviewer { get; set; }
        //public string applying_through { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "InterviewMode")]
        [Required(ErrorMessageResourceName = "InterviewModeRequired", ErrorMessageResourceType = typeof(Messages))]
        public string mode_of_interview { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Location")]
        [Required(ErrorMessageResourceName = "LocationRequired", ErrorMessageResourceType = typeof(Messages))]
        public string location { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Rating")]
        [Required(ErrorMessageResourceName = "RatingRequired", ErrorMessageResourceType = typeof(Messages))]
        public float rating { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Remark")]
        [Required(ErrorMessageResourceName = "RemarkRequired", ErrorMessageResourceType = typeof(Messages))]
        public string remark { get; set; }


        //enum status
        [EnumDataType(typeof(StatusType))]
        [Display(ResourceType = typeof(Labels), Name = "Status")]
        [Required(ErrorMessageResourceName = "StatusRequired", ErrorMessageResourceType = typeof(Messages))]
        public StatusType statusvalue { get; set; }

        public int status { get; set; }

       
        public string statusName { get; set; }


        //relationship with candidate :        
        public int candidateId { get; set; }

        //creating a list for interviewer names
        public List<UserModel> InterviewerNames { get; set; }
        public string allInterviewersNames { get; set; }

        //for jobapplication ID
        public int jobAppId { get; set; }

        //for jobrole
        [Display(ResourceType = typeof(Labels), Name = "JobRole")]
        [Required(ErrorMessageResourceName = "JobRoleRequired", ErrorMessageResourceType = typeof(Messages))]
        public string jobRole { get; set; }

        //for candidate data
        public CandidateViewModel candidate { get; set; }
        public string EncryptedJobApplicationId { get; set; }

    }
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
