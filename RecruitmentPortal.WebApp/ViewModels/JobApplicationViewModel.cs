using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobApplicationViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int round { get; set; }
        public string status { get; set; }
        public bool notified { get; set; }

        //for start date of candidate's interview process
        [Display(ResourceType = typeof(Labels), Name = "StartDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
        public DateTime start_date { get; set; }

        //for application rejection constraints

        [Display(ResourceType = typeof(Labels), Name = "Rejectdate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
        public DateTime rejection_date { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "RejectionReason")]
        public string rejection_reason { get; set; }

        //for date of acceptence of candidate application
        public DateTime date { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "AcceptDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
        public DateTime accept_date { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JoiningDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
        public DateTime joining_date { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "CommitmentMode")]
        public string commitment_mode { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "OfferedCTC")]
        public string offered_ctc { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Remark")]
        public string remarks { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "AllRounds")]
        public string allRounds { get; set; }


       


        //for data fetching
        public List<SchedulesViewModel> Schedules { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "CandidateName")]
        [Required(ErrorMessageResourceName = "CandidateNameRequired", ErrorMessageResourceType = typeof(Messages))]
        public string candidateName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JobTitle")]
        public string position { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JobRole")]
        public string job_Role { get; set; }

        //for accepting candidate application

        public bool flag_Accepted { get; set; }

        //for rejecting candidate application
        public bool flag_Rejected { get; set; }

        //flag for checking the edit joining date mode
        public bool flag_Edit { get; set; }

        //for navigation after editing the joining date from admin dashboard
        public bool editFromMenu { get; set; }

        //for interview status
        public string interview_Status { get; set; }

        //for storing candidate
        public CandidateViewModel candidate { get; set; }

        //for storing names of schedule done for rejected candidates
        public List<string> listOfRounds { get; set; }
        //relationship with candidate
        public int candidateId { get; set; }
        public string EncryptedJobId { get; set; }
        public string EncryptedCandidateId { get; set; }


       

    }
}
