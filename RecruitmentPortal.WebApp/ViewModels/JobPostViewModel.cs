using RecruitmentPortal.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobPostViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public string job_title { get; set; }
        public string location { get; set; }
        public string job_role { get; set; }
        public int vacancy { get; set; }
        public string job_type { get; set; }
        public string eligibility_criteria { get; set; }
        public string experience { get; set; }
        public string skills { get; set; }
        public bool isActive { get; set; }

        //for counter part of available jobs on Home page
        public int AvailableJobsCount { get; set; }

        //relationship with JobCategoryViewModel
        public int JobCategoryId { get; set; }

        //vacancy_fulfilled counter
        public bool vacancy_overflow { get; set; }
    }
}
