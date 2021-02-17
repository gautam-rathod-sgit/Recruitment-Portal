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
    public class JobCategoryRepository : Repository<JobCategory>, IJobCategoryRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public JobCategoryRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<JobCategory>> getAllJobCategoryWithJobPost()
        {
            var specs = new JobCategoryWithJobPostSpecification();
            return ApplySpecification(specs);
        }

        //public Task<IQueryable<JobCategory>> getJobCategoryByUserId(string id)
        //{
        //    return _context.JobCategory.Where(x => x.UserId == id);
        //}

        public async Task<JobCategory> getJobCategoryWithJobPostById(int id)
        {
            var specs = new JobCategoryWithJobPostSpecification(id);
            return (await GetAsync(specs)).FirstOrDefault();
        }
    }
}
