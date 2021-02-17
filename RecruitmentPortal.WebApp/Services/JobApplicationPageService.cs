using AutoMapper;
using AutoMapper.QueryableExtensions;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RecruitmentPortal.WebApp.Services
{
    public class JobApplicationPageService : IJobApplicationPage
    {
        //getting reference of upper layer repos
        private readonly IJobApplicationRepository _jobApplicationRepository;
        //getting mapper
        private readonly IMapper _mapper;

        public JobApplicationPageService(IJobApplicationRepository jobApplicationRepository, IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _mapper = mapper;
        }

        public async Task<JobApplicationViewModel> AddJobApplications(JobApplicationViewModel model)
        {
            var mapped = _mapper.Map<JobApplications>(model);
            await _jobApplicationRepository.Add(mapped);
            return model;
        }

        public async Task<JobApplicationViewModel> getJobApplicationById(int id)
        {
            return _mapper.Map<JobApplicationViewModel>(await _jobApplicationRepository.getById(id));
        }

        public async Task<IQueryable<JobApplicationViewModel>> getJobApplications()
        {
            IQueryable<JobApplicationViewModel> query1;
            var alist = await _jobApplicationRepository.getAll();
            //foreach(var item in alist)
            //{
            //    foreach(var s in item.Schedules)
            //    {
            //        StatusType status = Enum.Parse<StatusType>(s.status);
            //    }
            //}
            query1 = alist.ProjectTo<JobApplicationViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task UpdateJobApplication(JobApplicationViewModel model)
        {
            await _jobApplicationRepository.Update(_mapper.Map<JobApplications>(model));
        }
    }
}
