using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Plugins;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobCategoryController : BaseController
    {
        #region Private Members
        private readonly string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);

        IQueryable<JobCategoryViewModel> categorylist;
        private readonly IJobCategoryPage _jobCategoryPageservices;

        private readonly IJobPostPage _jobPostPageservices;

        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        private readonly RecruitmentPortalDbContext _dbContext;

        #endregion

        #region Constructor
        public JobCategoryController(IWebHostEnvironment environment,
            IJobCategoryPage jobCategoryPage,
              RecruitmentPortalDbContext dbContext,
            IJobPostPage jobPostPageservices,
             UserManager<ApplicationUser> userManager)
        {
            _jobCategoryPageservices = jobCategoryPage;
            _jobPostPageservices = jobPostPageservices;
            _environment = environment;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method retrieves list of Job Categories
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View();
        }
        /// <summary>
        /// method for getting all the job categories.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetJobCategories()
        {
            List<JobCategoryViewModel> list = null;
            try
            {
                categorylist = await _jobCategoryPageservices.getCategories();
                list = categorylist.ToList();

                foreach (var obj in list)
                {
                    obj.EncryptedId = RSACSPSample.EncodeServerName((obj.ID).ToString());
                }


                return Json(new { data = list });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = list });
            }

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
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Job Category already exists !!";
                        return View("Index");
                    }
                }

                if (model.ID != 0)
                {
                    return RedirectToAction("UpdateCategoryPost", model);
                }
                else
                {
                    model.isActive = true;
                    await _jobCategoryPageservices.AddNewCategory(model);

                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            return RedirectToAction("Index", "JobCategory");
        }

        /// <summary>
        /// This method is used for Updating Job Category [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateCategory(string id, bool status, bool editMode)
        {
            JobCategoryViewModel category = new JobCategoryViewModel();
            try
            {
                category = await _jobCategoryPageservices.getCategoryById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                if (status)
                {
                    //setting final results of deactivating
                    category.isActive = false;

                    //will deactivate all it's jobposts

                    //but first need to check if any job post is having active candidates? then only deactivate
                    bool counter = isCandidateActiveForJob(category.ID);
                    if (counter)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Action Denied .Candidate is Active with items to be Deleted !!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        using (_dbContext)
                        {
                            _dbContext.JobPost
                            .Where(x => x.JobCategoryId == category.ID)
                            .ToList()
                            .ForEach(a =>
                            {
                                a.isActive = false;
                            }
                            );
                            _dbContext.SaveChanges();


                            //---------------------------------------------------------------------------------

                            //foreach (var item in postlist)
                            //{
                            //    item.isActive = false;

                            //    //_dbContext.JobPost.Where(x => x.JobCategoryId == category.ID).ToList().ForEach(x => x.isActive = false);

                            //    await _jobPostPageservices.UpdateJobPost(item);
                            //    //(from p in _dbContext.JobPost
                            //    // where p.ID == category.ID
                            //    // select p).ToList()
                            //    //                .ForEach(x => x.isActive = false);

                            //    //_dbContext.SaveChanges();

                            //}---------------------------------------------------------------------------------

                        }
                    }
                    TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job Deactivated Successfully.";
                    return RedirectToAction("UpdateCategoryPost", category);
                }
                else
                {
                    if (editMode)
                    {
                        return View(category);
                    }
                    else
                    {
                        category.isActive = true;
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job Activated Successfully.";
                        return RedirectToAction("UpdateCategoryPost", category);
                    }

                }

            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            return View();
        }
        public bool isCandidateActiveForJob(int id)
        {
            List<JobApplications> jobAppList = new List<JobApplications>();
            bool isAvailable = false;
            var list = _dbContext.JobPost.Where(x => x.JobCategoryId == id).ToList();
            foreach (var item in list)
            {
                var listOfCandidates = _dbContext.JobPostCandidate.Where(x => x.job_Id == item.ID).ToList();
                foreach (var values in listOfCandidates)
                {
                    var data = _dbContext.jobApplications.Where(x => x.candidateId == values.candidate_Id).FirstOrDefault();
                    if (data != null)
                        jobAppList.Add(data);
                }
                var temp = jobAppList.Where(x => x.status == status_Pending).Any();
                if (temp)
                {
                    return true;
                }
            }
            return isAvailable;
        }

        /// <summary>
        /// This method is used for Updating Job Category [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateCategoryPost(JobCategoryViewModel model)
        {
            try
            {
                
                await _jobCategoryPageservices.UpdateCategory(model);

                //now also need to change the job title of its job post as it's dependent on job category name
                using (_dbContext)
                {
                    _dbContext.JobPost
                    .Where(x => x.JobCategoryId == model.ID)
                    .ToList()
                    .ForEach(a =>
                    {
                        a.job_title = model.job_category_name;
                    }
                    );
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            return RedirectToAction("Index", "JobCategory");
        }

        /// <summary>
        /// THIS METHOD IS USED TO FETCH DETAILS OF PARTICULAR JOB CATEGORY
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public ActionResult DetailsJobCategory(string id, string s, string activeCandidate)
        {

            JobCategoryViewModel model = new JobCategoryViewModel();
            model.EncryptedId = id;

            return View(model);
        }

        public async Task<IActionResult> GetCategoryDetails(string id)
        {
            List<JobPostViewModel> itemList = new List<JobPostViewModel>();
            JobCategoryViewModel item = new JobCategoryViewModel();

            try
            {
                item = await _jobCategoryPageservices.GetJobCategoryWithJobPostById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                foreach (var obj in item.JobPosts)
                {
                    obj.EncryptedId = RSACSPSample.EncodeServerName((obj.ID).ToString());
                    obj.categoryId = item.EncryptedId;
                }
                itemList = item.JobPosts;
                return Json(new { data = itemList });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = itemList });
            }
        }

        /// <summary>
        /// for Loading the popup for adding or edititng the category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RenderJobCategoryView(string id)
        {
            JobCategoryViewModel model = new JobCategoryViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                model = await _jobCategoryPageservices.getCategoryById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
            }
            return PartialView("_JobCategory", model);
        }
        #endregion
    }
}
