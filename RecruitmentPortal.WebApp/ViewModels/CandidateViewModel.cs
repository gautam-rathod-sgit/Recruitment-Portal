using Microsoft.AspNetCore.Http;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Models;
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


        [Required(ErrorMessage = "Required.")]
        [DisplayName("Candidate Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DisplayName("Email")]
        public string email { get; set; }

        public bool emailConfirmed { get; set; }
        public string token { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime dob { get; set; }


        [Required(ErrorMessage = "Required.")]
        public double phone { get; set; }

        //enum gender
        //[EnumDataType(typeof(GenderType))]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Required.")]
        [DisplayName("Experience")]
        public string experience { get; set; }

        [DisplayName("Total Experience")]
        public string total_experience { get; set; }

        [DisplayName("Relevent Experience")]
        public string relevent_experience { get; set; }

        [DisplayName("Current CTC")]
        public double current_ctc { get; set; }

        [DisplayName("Expected CTC")]
        public double expected_ctc { get; set; }

        [DisplayName("Candidate Name")]
        public string notice_period { get; set; }


        [Required(ErrorMessage = "Required.")]
        [DisplayName("HSC %")]
        public float hsc_perc { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DisplayName("SSC %")]
        public float ssc_perc { get; set; }

        [DisplayName("Applying Through")]
        public string applying_through { get; set; }


        //to be managed
        [DisplayName("Degree")]
        public string degree { get; set; }
        public int selectedDegree { get; set; }
        public int selectDept { get; set; }




        [Required(ErrorMessage = "Required.")]
        public string resume { get; set; }
        public IFormFile File { get; set; }


        //for schedules
        public List<SchedulesViewModel> Schedules { get; set; }

        //for carrying jobid to post method
        public int jobpostID { get; set; }

        //for getting the job position
        public string jobName { get; set; }
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
