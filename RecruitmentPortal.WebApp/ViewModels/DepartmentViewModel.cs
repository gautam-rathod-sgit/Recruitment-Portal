using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class DepartmentViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "DepartmentName")]
        [Required(ErrorMessageResourceName = "DeptNameRequired", ErrorMessageResourceType = typeof(Messages))]
        public string dept_name { get; set; }
        public bool isActive { get; set; }

        //relationship with DegreeViewModel
        public string DegreeId { get; set; }
    }
}
