using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class DegreeViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public string degree_name { get; set; }
        public bool isActive { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
    }
}
