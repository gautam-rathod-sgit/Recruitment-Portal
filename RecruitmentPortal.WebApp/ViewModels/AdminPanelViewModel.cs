using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class AdminPanelViewModel
    {
        public int ApplicationCount { get; set; }
        public int InterviewerCount { get; set; }
        public int ActiveApplicationCount { get; set; }
        public int SelectedCount { get; set; }

        //all upcoming schedules
        public List<SchedulesViewModel> upcoming_schedules { get; set; }
        //all notification for selected candidates 
        public List<JobApplicationViewModel> selected_application { get; set; }
    }
}
