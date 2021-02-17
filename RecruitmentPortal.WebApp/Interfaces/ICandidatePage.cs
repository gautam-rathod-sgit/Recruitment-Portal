using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface ICandidatePage
    {
        public Task<IQueryable<CandidateViewModel>> getCandidates();
        public Task<CandidateViewModel> getCandidateById(int id);
        public Task UpdateCandidate(CandidateViewModel model);
        public Task<CandidateViewModel> getCandidateByEmailId(string id);

        public Task<CandidateViewModel> AddNewCandidate(CandidateViewModel model);

        //using specification
        public Task<CandidateViewModel> getCandidateByIdWithSchedules(int cid);

    }
}
