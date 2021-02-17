using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class SchedulesUsersViewModel : BaseViewModel
    {
        public int scheduleId { get; set; }
        public string UserId { get; set; }

    }
}
