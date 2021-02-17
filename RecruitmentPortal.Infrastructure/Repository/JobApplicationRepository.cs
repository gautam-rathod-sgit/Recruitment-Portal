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
    public class JobApplicationRepository : Repository<JobApplications>, IJobApplicationRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public JobApplicationRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }
       
    }
}
