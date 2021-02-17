using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface ISchedulesUsersPage
    {
        public Task<IQueryable<SchedulesUsersViewModel>> getSchedulesUsers();
        public Task<SchedulesUsersViewModel> getSchedulesUsersId(int id);
        public Task<SchedulesUsersViewModel> AddNewSchedulesUsers(SchedulesUsersViewModel model);
        public Task UpdateSchedulesUsers(SchedulesUsersViewModel model);
        public Task DeleteScheduleUsers(int id);
    }
}
