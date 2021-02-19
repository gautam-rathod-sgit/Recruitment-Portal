using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobApplicationViewModel : BaseViewModel
    {
        //for ecryption
        [NotMapped]
        public string EncryptionId { get; set; }
        public int round { get; set; }
        public string status { get; set; }
        public DateTime joining_date { get; set; }
        public bool notified { get; set; }
        public string date { get; set; }

        //relationship with candidate
        public int candidateId { get; set; }


        //for data fetching
        public List<SchedulesViewModel> Schedules { get; set; }
        public string candidateName { get; set; }
        public string position { get; set; }

        //for storing candidate
        public CandidateViewModel candidate { get; set; }

    }
}
