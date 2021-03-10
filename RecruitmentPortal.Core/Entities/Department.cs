using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class Department : Entity
    {
        public string dept_name { get; set; }
        public bool isActive { get; set; }

        //relationship with degree
        [ForeignKey("Degree")]
        public int DegreeId { get; set; }
        public Degree Degree { get; set; }
    }
}
