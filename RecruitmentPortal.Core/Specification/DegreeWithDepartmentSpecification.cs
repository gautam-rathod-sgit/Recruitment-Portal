using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Specification.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Specification
{
    public class DegreeWithDepartmentSpecification : BaseSpecification<Degree>
    {
        public DegreeWithDepartmentSpecification()
         : base(null)
        {
            AddInclude(p => p.Departments);
        }
        public DegreeWithDepartmentSpecification(int id)
          : base(p => p.ID == id)
        {
            AddInclude(p => p.Departments);
        }
    }
}
