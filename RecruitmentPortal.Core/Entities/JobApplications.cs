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


        //for application rejection constraints
        public DateTime rejection_date { get; set; }
        public string rejection_reason { get; set; }

        //for start date of candidate's interview process
        public DateTime start_date { get; set; }

        //for date of acceptence of candidate application
        public DateTime accept_date { get; set; }
        public DateTime joining_date { get; set; }
        public string commitment_mode { get; set; }
        public string offered_ctc { get; set; }
        public string remarks { get; set; }



        //1-N relationship with Candidate
        [ForeignKey("Candidate")]
        public int candidateId { get; set; }


    }
}
