using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface ISchedulesPage
    {
        public Task<SchedulesViewModel> AddNewSchedules(SchedulesViewModel model);
        public Task<IQueryable<SchedulesViewModel>> GetSchedulesByUserId(string uid);
        public Task<IQueryable<SchedulesViewModel>> GetAllSchedules();
        public Task<SchedulesViewModel> GetSchedulesById(int id);
        public Task UpdateSchedule(SchedulesViewModel model);
        public Task DeleteSchedule(int id);
    }
}
