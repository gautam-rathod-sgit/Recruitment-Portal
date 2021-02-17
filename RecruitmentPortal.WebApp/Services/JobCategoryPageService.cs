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
    public class JobCategoryPageService : IJobCategoryPage
    {

        //getting reference of upper layer repos
        private readonly IJobCategoryRepository _jobCategoryRepository;
        //getting mapper
        private readonly IMapper _mapper;

        public JobCategoryPageService(IJobCategoryRepository jobCategoryRepository, IMapper mapper)
        {
            _jobCategoryRepository = jobCategoryRepository;
            _mapper = mapper;
        }


        //actions goes here
        public async Task<JobCategoryViewModel> AddNewCategory(JobCategoryViewModel model)
        {
            var mapped = _mapper.Map<JobCategory>(model);
            await _jobCategoryRepository.Add(mapped);
            return model;
        }

        public async Task DeleteCategory(int id)
        {
            await _jobCategoryRepository.Delete(await _jobCategoryRepository.getById(id));
        }

       

        public async Task<IQueryable<JobCategoryViewModel>> getCategories()
        {
            IQueryable<JobCategoryViewModel> query1;
            var alist = await _jobCategoryRepository.getAll();
            query1 = alist.ProjectTo<JobCategoryViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }
        public async Task UpdateCategory(JobCategoryViewModel model)
        {
            var mapped = _mapper.Map<JobCategory>(model);
            await _jobCategoryRepository.Update(mapped);
        }
        public async Task<JobCategoryViewModel> getCategoryById(int id)
        {
            var mapped = _mapper.Map<JobCategoryViewModel>(await _jobCategoryRepository.getById(id));
            return mapped;
        }

        //public Task<IQueryable<JobCategoryViewModel>> getCategoriesByUserId(string id)
        //{
        //    IQueryable<ArticleViewModel> query1;
        //    var alist = await _articleRepository.getArticlesByUserId(id);
        //    query1 = alist.ProjectTo<ArticleViewModel>(_mapper.ConfigurationProvider);
        //    return query1;
        //}





        //getting JobCategory with all JobPost by ID into 1 layer using specification
        public async Task<JobCategoryViewModel> GetJobCategoryWithJobPostById(int id)
        {
            var alist = await _jobCategoryRepository.getJobCategoryWithJobPostById(id);
            var mapped = _mapper.Map<JobCategoryViewModel>(alist);
            return mapped;
        }
        //getting ALL JobCategory with all JobPost into 1 layer using specification
        public async Task<IQueryable<JobCategoryViewModel>> GetAllJobCategoryWithJobPost()
        {
            IQueryable<JobCategoryViewModel> query;
            var alist = await _jobCategoryRepository.getAllJobCategoryWithJobPost();
            query = alist.ProjectTo<JobCategoryViewModel>(_mapper.ConfigurationProvider);
            return query;
        }





    }
}
