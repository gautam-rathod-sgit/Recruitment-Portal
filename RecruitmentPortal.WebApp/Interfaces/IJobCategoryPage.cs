using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IJobCategoryPage
    {
        public Task<IQueryable<JobCategoryViewModel>> getCategories();
        //public Task<IQueryable<JobCategoryViewModel>> getCategoriesByUserId(string id);
        public Task<JobCategoryViewModel> getCategoryById(int id);
        public Task<JobCategoryViewModel> AddNewCategory(JobCategoryViewModel model);
        public Task DeleteCategory(int id);
        public Task UpdateCategory(JobCategoryViewModel model);


        //using specification

        //for JobCategory with all JobPost
        public Task<JobCategoryViewModel> GetJobCategoryWithJobPostById(int id);
        //for all JobCategory with all JobPost
        public Task<IQueryable<JobCategoryViewModel>> GetAllJobCategoryWithJobPost();
    }
}
