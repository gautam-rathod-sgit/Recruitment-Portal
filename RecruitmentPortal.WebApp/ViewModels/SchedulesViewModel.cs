using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class SchedulesViewModel : BaseViewModel
    {
        public string candidate_name { get; set; }
        public string position { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime datetime { get; set; }
        public string time { get; set; }

        //enum round
        [EnumDataType(typeof(RoundType))]
        public RoundType roundValue { get; set; }
        public int round { get; set; }
        public string roundName { get; set; }

       
        [Required(ErrorMessage ="please choose the interviewers")]
        public List<string> Multiinterviewer { get; set; }
        //public string applying_through { get; set; }
        public string mode_of_interview { get; set; }
        public string location { get; set; }
        public float rating { get; set; }
        public string remark { get; set; }


        //enum status
        [EnumDataType(typeof(StatusType))]
        public StatusType statusvalue { get; set; }
        public int status { get; set; }
        public string statusName { get; set; }


        //relationship with candidate :        
        public int candidateId { get; set; }

        //creating a list for interviewer names
        public List<UserModel> InterviewerNames { get; set; }

        //for jobapplication ID
        public int jobAppId { get; set; }

        //for jobrole
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
