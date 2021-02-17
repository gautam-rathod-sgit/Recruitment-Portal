using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Repository
{
    public interface IDegreeRepository : IRepository<Degree>
    {
        //getting degree with all dept by AID using SPECIFICATIONS
        public Task<Degree> getDegreeWithDeptById(int id);

        //getting All JobCategory with all JobPost by AID using SPECIFICATIONS
        public Task<IQueryable<Degree>> getAllDegreeWithDepartment();

    }
}
