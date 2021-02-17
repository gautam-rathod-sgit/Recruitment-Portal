using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Plugins;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobCategoryController : Controller
    {

        IQueryable<JobCategoryViewModel> categorylist;
        private readonly IJobCategoryPage _jobCategoryPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        public JobCategoryController(IWebHostEnvironment environment, IJobCategoryPage jobCategoryPage,
             UserManager<ApplicationUser> userManager)
        {
            _jobCategoryPageservices = jobCategoryPage;
            _environment = environment;
            _userManager = userManager;
        }


        /// <summary>
        /// This method retrieves list of Job Categories
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            categorylist = await _jobCategoryPageservices.getCategories();
            return View(categorylist);
        }

        /// <summary>
        /// This method is used for adding new category [GET]
        /// </summary>
        /// <returns></returns>
        public IActionResult AddNewCategory()
        {
            return View();
        }

        /// <summary>
        /// This method is used for adding new category [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewCategory(JobCategoryViewModel model)
        {
            categorylist = await _jobCategoryPageservices.getCategories();
            try
            {
                foreach (var item in categorylist)
                {
                    if (item.job_category_name == model.job_category_name)
                    {
                        ViewData["msg"] = model.job_category_name;
                        return View();
                    }
                }
                await _jobCategoryPageservices.AddNewCategory(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index", "JobCategory");
        }

        /// <summary>
        /// This method is used for Deleting Job Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _jobCategoryPageservices.DeleteCategory(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index", "JobCategory");
        }

        /// <summary>
        /// This method is used for Updating Job Category [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateCategory(int id)
        {
            JobCategoryViewModel category = new JobCategoryViewModel();
            try
            {
                category = await _jobCategoryPageservices.getCategoryById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(category);
        }

        /// <summary>
        /// This method is used for Updating Job Category [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(JobCategoryViewModel model)
        {
            try
            {
                await _jobCategoryPageservices.UpdateCategory(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index", "JobCategory");
        }

        /// <summary>
        /// THIS METHOD IS USED TO FETCH DETAILS OF PARTICULAR JOB CATEGORY
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailsJobCategory(int id, string s)
        {
            if (s != null)
                ViewData["msg"] = s;
            JobCategoryViewModel item = new JobCategoryViewModel();

            try
            {
                item = await _jobCategoryPageservices.GetJobCategoryWithJobPostById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(item);
        }
    }
}
