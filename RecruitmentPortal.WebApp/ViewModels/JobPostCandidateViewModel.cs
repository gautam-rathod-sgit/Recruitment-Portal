using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobPostCandidateViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int job_Id { get; set; }
        public int candidate_Id { get; set; }

    }
}
