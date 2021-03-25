using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class scheduleTableViewModel
    {
        public string roundName { get; set; }
        public float rating { get; set; }
        public string remark { get; set; }
        public string statusName { get; set; }
        public DateTime datetime { get; set; }
        public List<UserModel> InterviewerNames { get; set; }

    }
}
