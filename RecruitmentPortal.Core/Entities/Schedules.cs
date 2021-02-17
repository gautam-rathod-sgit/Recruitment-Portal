using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class Schedules : Entity
    {
        public string candidate_name { get; set; }
        public string position { get; set; }
        public DateTime datetime { get; set; }
        public int round { get; set; }
        public string applying_through { get; set; }
        public string mode_of_interview { get; set; }
        public string location { get; set; }
        public float rating { get; set; }
        public string remark { get; set; }
        public int status { get; set; }

        //relationship with candidate : 
        [ForeignKey("Candidate")]
        public int candidateId { get; set; }
        public Candidate Candidate { get; set; }

        //relationship with SchedulesUsers
        public IList<SchedulesUsers> SchedulesUsers { get; set; }

       

    }
}
