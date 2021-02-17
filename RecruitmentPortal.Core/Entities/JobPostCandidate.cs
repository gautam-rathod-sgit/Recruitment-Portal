using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class JobPostCandidate : Entity
    {
        [ForeignKey("JobPost")]
        public int job_Id { get; set; }
        public JobPost JobPost { get; set; }


        public int candidate_Id { get; set; }
        [ForeignKey(" candidate_Id")]
        public Candidate Candidate { get; set; }
    }
}
