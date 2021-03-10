using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IJobApplicationPage
    {
        public Task<JobApplicationViewModel> AddJobApplications(JobApplicationViewModel model);
        public Task<IQueryable<JobApplicationViewModel>> getJobApplications();
        public Task<JobApplicationViewModel> getJobApplicationById(int id);
        public Task UpdateJobApplication(JobApplicationViewModel model);
        public string getInterviewStatusForApplication(int id);
    }
}
