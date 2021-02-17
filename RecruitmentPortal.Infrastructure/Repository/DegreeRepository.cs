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
    public class DegreeRepository : Repository<Degree>, IDegreeRepository
    {
        private readonly RecruitmentPortalDbContext _context;
        public DegreeRepository(RecruitmentPortalDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<Degree>> getAllDegreeWithDepartment()
        {
            var specs = new DegreeWithDepartmentSpecification();
            return ApplySpecification(specs);
        }

        public async Task<Degree> getDegreeWithDeptById(int id)
        {
            var specs = new DegreeWithDepartmentSpecification(id);
            return (await GetAsync(specs)).FirstOrDefault();
        }
    }
}
