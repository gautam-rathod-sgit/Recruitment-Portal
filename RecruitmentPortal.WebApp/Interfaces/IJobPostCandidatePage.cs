using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IJobPostCandidatePage
    {
        public  Task<JobPostCandidateViewModel> AddNewJobPostCandidate(JobPostCandidateViewModel model);
        public Task<IQueryable<JobPostCandidateViewModel>> getJobPostCandidateAll();
    }
}
