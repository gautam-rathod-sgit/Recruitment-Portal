using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class JobPost : Entity
    {
        public string job_title { get; set; }
        public string location { get; set; }
        public string job_role { get; set; }
        public int vacancy { get; set; }
        public string job_type { get; set; }
        public string eligibility_criteria { get; set; }
        public string experience { get; set; }
        public string skills { get; set; }

        //relationship with job_category
        [ForeignKey("JobCategory")]
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }

        //relationship with JobPostCandidate
        public List<JobPostCandidate> JobPostCandidate { get; set; }

    }
}
