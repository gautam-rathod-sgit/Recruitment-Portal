using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class DepartmentViewModel
    {
        //custom ID
        public int ID { get; set; }
        public string dept_name { get; set; }

        //relationship with DegreeViewModel
        public int DegreeId { get; set; }
    }
}
