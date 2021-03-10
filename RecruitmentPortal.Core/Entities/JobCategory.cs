using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class JobCategory : Entity
    {
        public string job_category_name { get; set; }
        public bool isActive { get; set; }

        //relationship with job_post
        public List<JobPost> JobPosts { get; set; }

    }
}
