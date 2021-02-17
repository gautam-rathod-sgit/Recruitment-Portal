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
    public class JobPostPageService : IJobPostPage
    {
        //getting reference of upper layer repos
        private readonly IJobPostRepository _jobPostRepository;
        //getting mapper
        private readonly IMapper _mapper;
        public JobPostPageService(IJobPostRepository jobPostRepository, IMapper mapper)
        {
            _jobPostRepository = jobPostRepository;
            _mapper = mapper;
        }

        public async Task<JobPostViewModel> AddNewJobPost(JobPostViewModel model)
        {
            var mapped = _mapper.Map<JobPost>(model);
            await _jobPostRepository.Add(mapped);
            return model;
        }

        public async Task DeleteJobPost(int id)
        {
            await _jobPostRepository.Delete(await _jobPostRepository.getById(id));
        }

        public async Task<IQueryable<JobPostViewModel>> getJobPost()
        {
            IQueryable<JobPostViewModel> query1;
            var item = await _jobPostRepository.getAll();
            query1 = item.ProjectTo<JobPostViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task<JobPostViewModel> getJobPostById(int id)
        {
            return _mapper.Map<JobPostViewModel>(await _jobPostRepository.getById(id));
        }

        public async Task UpdateJobPost(JobPostViewModel model)
        {
            await _jobPostRepository.Update(_mapper.Map<JobPost>(model));
        }
    }
}
