using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class InterviewerPanelViewModel
    {
        public int PendingScheduleCount { get; set; }
        public int CompletedScheduleCount { get; set; }

        //all Pending schedules
        public List<SchedulesViewModel> pending_schedules { get; set; }
    }
}
