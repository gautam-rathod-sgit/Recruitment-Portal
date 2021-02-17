using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobPostCandidateViewModel : BaseViewModel
    {
       
        public int job_Id { get; set; }
        public int candidate_Id { get; set; }

    }
}
