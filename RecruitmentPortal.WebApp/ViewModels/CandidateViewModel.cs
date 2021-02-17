using Microsoft.AspNetCore.Http;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace RecruitmentPortal.WebApp.ViewModels
{
    public class CandidateViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }

        public bool emailConfirmed { get; set; }
        public string token { get; set; }

        [Required(ErrorMessage = "Required.")]
        public double phone { get; set; }

        //enum gender
        //[EnumDataType(typeof(GenderType))]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Required.")]
        public string experience { get; set; }

        [Required(ErrorMessage = "Required.")]
        public float hsc_perc { get; set; }

        [Required(ErrorMessage = "Required.")]
        public float ssc_perc { get; set; }




        //to be managed
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
    }
}
