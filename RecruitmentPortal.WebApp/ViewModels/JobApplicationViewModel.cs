using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobApplicationViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int round { get; set; }
        public string status { get; set; }
        public bool notified { get; set; }

        //for start date of candidate's interview process
        public DateTime start_date { get; set; }

        //for application rejection constraints
        public DateTime rejection_date { get; set; }
        public string rejection_reason { get; set; }

        //for date of acceptence of candidate application
        public DateTime accept_date { get; set; }

        public DateTime joining_date { get; set; }
        public DateTime date { get; set; }

        public string commitment_mode { get; set; }
        public string offered_ctc { get; set; }
        public string remarks { get; set; }


        //relationship with candidate
        public int candidateId { get; set; }


        //for data fetching
        public List<SchedulesViewModel> Schedules { get; set; }
        public string candidateName { get; set; }
        public string position { get; set; }

        public string job_Role { get; set; }

        //for accepting candidate application

        public bool flag_Accepted { get; set; }

        //for rejecting candidate application
        public bool flag_Rejected { get; set; }

        //flag for checking the edit joining date mode
        public bool flag_Edit { get; set; }

        //for navigation after editing the joining date from admin dashboard
        public bool editFromMenu { get; set; }

        //for interview status
        public string interview_Status { get; set; }

        //for storing candidate
        public CandidateViewModel candidate { get; set; }

        //for storing names of schedule done for rejected candidates
        public List<string> listOfRounds { get; set; }
        public string EncryptedJobId { get; set; }
        public string EncryptedCandidateId { get; set; }


        [Display]
        public string allRounds { get; set; }

    }
}
