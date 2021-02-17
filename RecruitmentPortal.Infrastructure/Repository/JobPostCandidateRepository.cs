using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Repository
{
    public class JobPostCandidateRepository : Repository<JobPostCandidate>, IJobPostCandidateRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public JobPostCandidateRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
