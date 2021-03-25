using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobPostController : BaseController
    {
        #region Private Members
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        IQueryable<JobPostViewModel> postlist;
        private readonly IJobPostPage _jobPostPageservices;

        //for getting name of job category
        private readonly IJobCategoryPage _jobCategoryPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        private readonly RecruitmentPortalDbContext _dbContext;
        #endregion

        #region Constructor
        public JobPostController(IWebHostEnvironment environment,
             IJobPostPage jobPostPageservices,
             RecruitmentPortalDbContext dbContext,
             IJobCategoryPage jobCategoryPageservices,
             UserManager<ApplicationUser> userManager)
        {
            _jobPostPageservices = jobPostPageservices;
            _environment = environment;
            _jobCategoryPageservices = jobCategoryPageservices;
            _userManager = userManager;
            _dbContext = dbContext;

        }
        #endregion

        #region Private Methods
        public async Task<IActionResult> Index()
        {
            return View(await _jobPostPageservices.getJobPost());
        }


        /// <summary>
        /// THIS METHOD IS USED FOR ADDING NEW JOBPOST [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddNewJobPost(string id, bool toIndex)
        {

            JobPostViewModel model = new JobPostViewModel();
            try
            {
                if (toIndex == true)
                {
                    ViewBag.toIndex = toIndex;
                }
                int decrypted_id = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
                //fetch job_title also automatically.
                model.job_title = _jobCategoryPageservices.getCategoryById(decrypted_id).Result.job_category_name;
                model.JobCategoryId = decrypted_id;
                model.isActive = true;
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
        public async Task<IActionResult> AddNewJobPost(JobPostViewModel model, string submit)
        {
            postlist = await _jobPostPageservices.getJobPost();
            try
            {
                foreach (var item in postlist)
                {
                    if (item.job_role == model.job_role && item.job_title == model.job_title)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Job Post already exists !!";
                        return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((item.ID).ToString()), s = TempData["msg"] });
                    }
                }
                model.isActive = true;
                var LastAddedJob = await _jobPostPageservices.AddNewJobPost(model);


                switch (submit)
                {
                    case "Save":
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job updated Successfully.";
                        return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((model.JobCategoryId).ToString()) });

                    case "Save & Continue":
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job updated Successfully.";
                        return RedirectToAction("UpdateJobPost", "JobPost", new
                        {
                            id = RSACSPSample.EncodeServerName((LastAddedJob.ID).ToString()),
                            categoryId = RSACSPSample.EncodeServerName((LastAddedJob.JobCategoryId).ToString()),
                            editMode = true,

                        });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((model.JobCategoryId).ToString()) });
        }

        /// <summary>
        /// THIS METHOD IS USED FOR DELETING PARTICULAR JOBPOST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        //public async Task<IActionResult> DeleteJobPost(string id, string cid)
        //{
        //    int category_id = Convert.ToInt32(RSACSPSample.DecodeServerName(cid));
        //    try
        //    {
        //        await _jobPostPageservices.DeleteJobPost(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((category_id).ToString()) });
        //}




        /// <summary>
        /// THIS METHOD IS USED FOR UPDATING PARTICULAR JOBPOST [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateJobPost(string id, string categoryId, bool status, bool editMode)
        {
            JobPostViewModel item = new JobPostViewModel();
            try
            {
                item = await _jobPostPageservices.getJobPostById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                //item.JobCategoryId = Convert.ToInt32(RSACSPSample.DecodeServerName(categoryId));

                if (status == true)
                {
                    //checking if any candidate is active with this job
                    bool isAvailable = _dbContext.JobPostCandidate.Where(x => x.job_Id == Convert.ToInt32(RSACSPSample.DecodeServerName(id))).Any();
                    bool isActiveCandidate = false;
                    List<JobApplications> jobAppList = new List<JobApplications>();

                    if (isAvailable)
                    {
                        var listOfCandidates = _dbContext.JobPostCandidate.Where(x => x.job_Id == Convert.ToInt32(RSACSPSample.DecodeServerName(id))).ToList();

                        foreach (var values in listOfCandidates)
                        {
                            var data = _dbContext.jobApplications.Where(x => x.candidateId == values.candidate_Id).FirstOrDefault();
                            if (data != null)
                                jobAppList.Add(data);
                        }

                        var temp = jobAppList.Where(x => x.status == status_Pending).Any();

                        if (temp)
                        {
                            isActiveCandidate = true;
                        }
                    }
                    if (isActiveCandidate)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = Messages.DeactivationFailed;
                        return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = categoryId });
                    }
                    else
                    {
                        item.isActive = false;
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job Deactivated Successfully."; 

                        return RedirectToAction("UpdateJobPostPost", "JobPost", item);
                    }
                }
                else
                {
                    if (editMode)
                    {
                        return View(item);
                    }
                    else
                    {
                        item.isActive = true;
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job Activated Successfully.";
                        return RedirectToAction("UpdateJobPostPost", "JobPost", item);
                    }

                }

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

        public async Task<IActionResult> UpdateJobPostPost(JobPostViewModel model, string submit, bool vacancy_overflow = false)
        {
            try
            {
                if (vacancy_overflow)
                {
                    CandidateViewModel candidateModel = new CandidateViewModel();
                    JobPostViewModel jobPostModel = new JobPostViewModel();
                    jobPostModel = JsonConvert.DeserializeObject<JobPostViewModel>((string)TempData["jobpost"]);
                    candidateModel = JsonConvert.DeserializeObject<CandidateViewModel>((string)TempData["candidate"]);

                    //deactivating job application
                    jobPostModel.isActive = false;

                    await _jobPostPageservices.UpdateJobPost(jobPostModel);

                    TempData["candidateForEmailConfirmation"] = JsonConvert.SerializeObject(candidateModel);
                    //redirecting to email confirmation process
                    return RedirectToAction("SendOTPToMail", "Candidate", new { vacancy_overflow = true });
                }
                else
                {
                    await _jobPostPageservices.UpdateJobPost(model);

                    switch (submit)
                    {
                        case "Save":
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job updated Successfully.";
                            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((model.JobCategoryId).ToString()) });

                        case "Save & Continue":
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Job updated Successfully.";
                            return RedirectToAction("UpdateJobPost", "JobPost", new
                            {
                                id = RSACSPSample.EncodeServerName((model.ID).ToString()),
                                categoryId = RSACSPSample.EncodeServerName((model.JobCategoryId).ToString()),
                                editMode = true
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "JobCategory");
            }

            return RedirectToAction("DetailsJobCategory", "JobCategory", new { id = RSACSPSample.EncodeServerName((model.JobCategoryId).ToString()) });
        }
        #endregion
    }
}
