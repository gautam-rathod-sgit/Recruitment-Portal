using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.Core.Specification;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Infrastructure.Repository
{
    public class SchedulesRepository : Repository<Schedules>, ISchedulesRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public SchedulesRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Schedules> getSchedulesByCandidateId(int Cid)
        {
            //var specs = new CandidateWithSchedulesSpecification(Cid);
            //return (await GetAsync(specs)).FirstOrDefault();
            return await _context.Schedules.AsNoTracking().FirstOrDefaultAsync(x => x.candidateId == Cid);
        }
        public async Task<IQueryable<Schedules>> getSchedulesByUserId(string id)
        {
            var all = (from element in _context.SchedulesUsers select element).Where(x => x.UserId == id).ToList();
            IQueryable<Schedules> data = null;
            List<Schedules> temp = new List<Schedules>();

            foreach (var item in all)
            {
                var y = _context.Schedules.AsNoTracking().FirstOrDefault(x => x.ID == item.scheduleId);
                temp.Add(y);
            }
            data = temp.AsQueryable();
            return data;
        }
    }
}
