using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobCategoryViewModel : EncyptionHelperModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "JobCategory")]
        [RegularExpression(CommonHelper.RegexAlphaNumSpecialChar, ErrorMessageResourceName = "ValidTextRequired", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "JobCategoryRequired", ErrorMessageResourceType = typeof(Messages))]
        public string job_category_name { get; set; }
        public bool isActive { get; set; }

        //relationship with JobPostViewModel
        public List<JobPostViewModel> JobPosts { get; set; }
        //public string ReturnUrl { get; set; }

        //defining viewbag
        public object ViewBag { get; }
    }
}
