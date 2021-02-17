using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Specification.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Specification
{
    public class JobCategoryWithJobPostSpecification : BaseSpecification<JobCategory>
    {
        public JobCategoryWithJobPostSpecification()
         : base(null)
        {
            AddInclude(p => p.JobPosts);
        }
        public JobCategoryWithJobPostSpecification(int id)
          : base(p => p.ID == id)
        {
            AddInclude(p => p.JobPosts);
        }
    }
}
