using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data.Enum;
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

        [DisplayName("Candidate Name")]
        public string candidate_name { get; set; }

        [DisplayName("Candidate Name")]
        public string position { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Candidate Name")]
        public DateTime datetime { get; set; }

        public string time { get; set; }

        //enum round
        [EnumDataType(typeof(RoundType))]
        public RoundType roundValue { get; set; }
        public int round { get; set; }

        [DisplayName("Candidate Name")]
        public string roundName { get; set; }

       
        [Required(ErrorMessage ="please choose the interviewers")]
        public List<string> Multiinterviewer { get; set; }
        //public string applying_through { get; set; }

        [DisplayName("Interview Mode")]
        public string mode_of_interview { get; set; }

        [DisplayName("Location")]
        public string location { get; set; }

        [DisplayName("Rating")]
        public float rating { get; set; }


        [DisplayName("Remarks")]
        public string remark { get; set; }


        //enum status
        [EnumDataType(typeof(StatusType))]
        public StatusType statusvalue { get; set; }

        public int status { get; set; }

        [DisplayName("Status")]
        public string statusName { get; set; }


        //relationship with candidate :        
        public int candidateId { get; set; }

        //creating a list for interviewer names
        public List<UserModel> InterviewerNames { get; set; }

        //for jobapplication ID
        public int jobAppId { get; set; }

        //for jobrole
        [DisplayName("Job Role")]
        public string jobRole { get; set; }

        //for candidate data
        public CandidateViewModel candidate { get; set; }

    }
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
