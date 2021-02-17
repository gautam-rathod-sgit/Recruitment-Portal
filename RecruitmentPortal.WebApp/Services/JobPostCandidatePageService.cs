using AutoMapper;
using AutoMapper.QueryableExtensions;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Services
{
    public class JobPostCandidatePageService : IJobPostCandidatePage
    {
        //getting reference of upper layer repos
        private readonly IJobPostCandidateRepository _jobPostCandidateRepository;
        //getting mapper
        private readonly IMapper _mapper;
        public JobPostCandidatePageService(IJobPostCandidateRepository jobPostCandidateRepository, IMapper mapper)
        {
            _jobPostCandidateRepository = jobPostCandidateRepository;
            _mapper = mapper;
        }

        public async Task<JobPostCandidateViewModel> AddNewJobPostCandidate(JobPostCandidateViewModel model)
        {
            var mapped = _mapper.Map<JobPostCandidate>(model);
            await _jobPostCandidateRepository.Add(mapped);
            return model;
        }

        public async Task<IQueryable<JobPostCandidateViewModel>> getJobPostCandidateAll()
        {
            IQueryable<JobPostCandidateViewModel> query1;
            var item = await _jobPostCandidateRepository.getAll();
            query1 = item.ProjectTo<JobPostCandidateViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }
    }
}
