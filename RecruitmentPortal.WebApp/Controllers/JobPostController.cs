using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobPostController : Controller
    {
        IQueryable<JobPostViewModel> postlist;
        private readonly IJobPostPage _jobPostPageservices;

        //for getting name of job category
        private readonly IJobCategoryPage _jobCategoryPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        public JobPostController(IWebHostEnvironment environment,
             IJobPostPage jobPostPageservices,
             IJobCategoryPage jobCategoryPageservices,
             UserManager<ApplicationUser> userManager)
        {
            _jobPostPageservices = jobPostPageservices;
            _environment = environment;
            _jobCategoryPageservices = jobCategoryPageservices;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _jobPostPageservices.getJobPost());
        }


        /// <summary>
        /// THIS METHOD IS USED FOR ADDING NEW JOBPOST [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddNewJobPost(int id)
        {
            JobPostViewModel model = new JobPostViewModel();
            try
            {
                //fetch job_title also automatically.
                model.job_title = _jobCategoryPageservices.getCategoryById(id).Result.job_category_name;
                model.JobCategoryId = id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(model);
        }

        /// <summary>
        /// THIS METHOD IS USED FOR ADDING NEW JOBPOST [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewJobPost(JobPostViewModel model)
        {
            postlist = await _jobPostPageservices.getJobPost();
            try
            {
                foreach (var item in postlist)
                {
                    if (item.job_role == model.job_role && item.job_title == model.job_title)
                    {
                        TempData["msg"] = model.job_role;
                        return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = model.JobCategoryId, s = TempData["msg"] });
                    }
                }
                await _jobPostPageservices.AddNewJobPost(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = model.JobCategoryId });
        }

        /// <summary>
        /// THIS METHOD IS USED FOR DELETING PARTICULAR JOBPOST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteJobPost(int id, int cid)
        {
            try
            {
                await _jobPostPageservices.DeleteJobPost(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = cid });
        }

        /// <summary>
        /// THIS METHOD IS USED FOR UPDATING PARTICULAR JOBPOST [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateJobPost(int id, int cid)
        {
            JobPostViewModel item = new JobPostViewModel();
            try
            {
                item = await _jobPostPageservices.getJobPostById(id);
                item.JobCategoryId = cid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(item);
        }

        /// <summary>
        /// THIS METHOD IS USED FOR UPDATING PARTICULAR JOBPOST [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateJobPost(JobPostViewModel model)
        {
            try
            {
                await _jobPostPageservices.UpdateJobPost(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = model.JobCategoryId });
        }
    }
}
