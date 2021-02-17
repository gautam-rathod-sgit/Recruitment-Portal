using RecruitmentPortal.WebApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.ViewModels
{
    public class JobCategoryViewModel : BaseViewModel
    {
        public string job_category_name { get; set; }

        //relationship with JobPostViewModel
        public List<JobPostViewModel> JobPosts { get; set; }
        //public string ReturnUrl { get; set; }

        //defining viewbag
        public object ViewBag { get; }
    }
}
