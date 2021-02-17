using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Repository
{
    public interface ISchedulesRepository : IRepository<Schedules>
    {
        //getting JobCategory with all JobPost by AID using SPECIFICATIONS
        public Task<Schedules> getSchedulesByCandidateId(int id);
        public Task<IQueryable<Schedules>> getSchedulesByUserId(string id);

    }
}
