using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Repository
{
    public interface IJobCategoryRepository : IRepository<JobCategory>
    {

        //getting JobCategory with all JobPost by AID using SPECIFICATIONS
        public Task<JobCategory> getJobCategoryWithJobPostById(int id);

        //getting All JobCategory with all JobPost by AID using SPECIFICATIONS
        public Task<IQueryable<JobCategory>> getAllJobCategoryWithJobPost();
    }
}
