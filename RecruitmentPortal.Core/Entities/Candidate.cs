using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class Candidate : Entity
    {
        public DateTime apply_date { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public bool emailConfirmed { get; set; }
        public DateTime dob { get; set; }
        public double phone { get; set; }
        public string Gender { get; set; }
        public string experience { get; set; }
        public string total_experience { get; set; }
        public string relevent_experience { get; set; }
        public double current_ctc { get; set; }
        public double expected_ctc { get; set; }
        public string notice_period { get; set; }
        public float hsc_perc { get; set; }
        public float ssc_perc { get; set; }
        public string degree { get; set; }
        public bool is_InitReject { get; set; }
        public string applying_through { get; set; }
        public string resume { get; set; }

        //relationship with Schedules
        public List<Schedules> Schedules { get; set; }

        //1-N relationship with JobApplications
        public List<JobApplications> JobApplications { get; set; }



        //relationship with JobPostCandidate
        public List<JobPostCandidate> JobPostCandidate { get; set; }

    }
}
