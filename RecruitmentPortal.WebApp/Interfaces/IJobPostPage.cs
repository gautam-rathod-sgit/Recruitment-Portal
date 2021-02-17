using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IJobPostPage
    {
        public Task<IQueryable<JobPostViewModel>> getJobPost();
        public Task<JobPostViewModel> getJobPostById(int id);
        public Task<JobPostViewModel> AddNewJobPost(JobPostViewModel model);
        public Task DeleteJobPost(int id);
        public Task UpdateJobPost(JobPostViewModel model);
    }
}
