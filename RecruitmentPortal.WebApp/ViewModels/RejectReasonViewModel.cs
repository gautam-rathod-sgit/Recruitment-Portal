using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class RejectReasonViewModel
    {
        //for CandidateID
        public string CandidateId { get; set; }
        //for jobApplicationID
        public int JobAppId { get; set; }
        public string rejection_reason { get; set; }
    }
}
