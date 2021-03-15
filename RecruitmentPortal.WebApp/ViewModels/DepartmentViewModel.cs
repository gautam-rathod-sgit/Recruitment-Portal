using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class DepartmentViewModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public string dept_name { get; set; }
        public bool isActive { get; set; }

        //relationship with DegreeViewModel
        public int DegreeId { get; set; }
    }
}
