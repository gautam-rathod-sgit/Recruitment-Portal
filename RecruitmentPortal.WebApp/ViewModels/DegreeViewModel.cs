using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class DegreeViewModel : BaseViewModel
    {
        public string degree_name { get; set; }
        public bool isActive { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
    }
}
