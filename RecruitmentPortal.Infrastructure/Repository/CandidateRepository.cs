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
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public CandidateRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Candidate> getCandidateByEmailId(string id)
        {
            //return  _context.Candidate.Where(x => x.email == id).FirstOrDefault();
            return await _context.Candidate.AsNoTracking().FirstOrDefaultAsync(x => x.email == id);
        }

        public async Task<Candidate>getCandidateByIdWithSchedules(int id)
        {
            var specs = new CandidateWithSchedulesSpecification(id);
            return (await GetAsync(specs)).FirstOrDefault();
        }
    }
}
