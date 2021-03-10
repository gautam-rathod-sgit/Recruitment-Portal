using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class Degree : Entity
    {
        public string degree_name { get; set; }
        public bool isActive { get; set; }

        //relationship with department
        public List<Department> Departments { get; set; }
    }
}
