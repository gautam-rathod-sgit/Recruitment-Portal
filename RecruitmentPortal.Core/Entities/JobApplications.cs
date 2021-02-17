using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class JobApplications : Entity
    {
        public int round { get; set; }
        public string status { get; set; }
        public bool notified { get; set; }
        public DateTime joining_date { get; set; }

        //1-N relationship with Candidate
        [ForeignKey("Candidate")]
        public int candidateId { get; set; }


    }
}
