﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.Infrastructure.Repository;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Security;
using RecruitmentPortal.WebApp.Services;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region private variables
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private string status_Complete = Enum.GetName(typeof(JobApplicationStatus), 2);
        readonly int reqValue = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Completed"));
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly IJobPostPage _jobPostPage;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISchedulesPage _schedulesPage;
        private readonly IJobApplicationPage _jobApplicationPage;
        private readonly ICandidatePage _candidatePage;
        private readonly IDataProtector _Protector;
        #endregion

        #region Constructor
        public HomeController(ICandidatePage candidatePage,
            IJobApplicationPage jobApplicationPage,
            ISchedulesPage schedulesPage,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            RecruitmentPortalDbContext dbContext,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings,
            IJobPostPage jobPostPage)
        {
            _logger = logger;
            _dbContext = dbContext;
            _jobApplicationPage = jobApplicationPage;
            _schedulesPage = schedulesPage;
            _candidatePage = candidatePage;
            _userManager = userManager;
            _jobPostPage = jobPostPage;
            _Protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.JobPostIdRouteValue);
        }
        //public async Task<IActionResult> DeactivateJobPost(JobPostViewModel jobPostModel)
        //{
        #endregion

        //    await _jobPostPageservices.UpdateJobPost(jobPostModel);
        //    return RedirectToAction("SendOTPToMail", model);
        //}

        #region Public Methods
        /// <summary>
        /// This method fetches Home Page
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="sortOrder"></param>
        /// <param name="pageNumber"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        //public async Task<IActionResult> Index(string SearchString, string sortOrder, int? pageNumber, string s)
        //{
        //    if (User.IsInRole("Admin"))
        //    {
        //        return RedirectToAction("AdminIndex", "Home");
        //    }
        //    if (User.IsInRole("Interviewer"))
        //    {
        //        return RedirectToAction("InterviewerIndex", "Home");
        //    }
        //    else
        //    {
        //        ////checking if vacancy of job completed or not
        //        var vacancy = _dbContext.JobPost.FirstOrDefault(x => x.ID == newModel.job_Id).vacancy;
        //        var count_post = _dbContext.JobPostCandidate.Where(x => x.job_Id == newModel.job_Id).Count();

        //        if (vacancy <= count_post)
        //        {
        //            var jobPostModel = await JobPostPageService.getJobPostById(newModel.job_Id);
        //            return RedirectToAction("DeactivateJobPost", new { model, jobPostModel });
        //        }


        //        //for candidate conflict
        //        if (s != null)
        //            ViewData["msg"] = s;

        //        ViewData["CurrentSort"] = sortOrder;
        //        ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_title" : "";
        //        ViewData["LocationSortParm"] = String.IsNullOrEmpty(sortOrder) ? "location" : "";
        //        ViewData["JobRoleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_role" : "";
        //        ViewData["JobTypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_type" : "";


        //        IQueryable<JobPostViewModel> plist = await _jobPostPage.getJobPost();
        //        //IEnumerable<JobPostViewModel> newlist;
        //        //newlist = plist.ToList();
        //        //foreach(var item in newlist)
        //        //{
        //        //    item.EncryptionId = _Protector.Protect(item.ID.ToString());
        //        //}
        //        //plist = newlist.AsQueryable();
        //        //foreach(var item in plist)
        //        //{
        //        //    item.EncryptionId = _Protector.Protect(item.ID.ToString());
        //        //}
        //        //plist = plist.Select(c =>
        //        //{
        //        //    c.EncryptionId = _Protector.Protect(c.ID.ToString());
        //        //});
        //        try
        //        {
        //            if (!String.IsNullOrEmpty(SearchString))
        //            {
        //                plist = plist.Where(s => s.job_title.ToUpper().Contains(SearchString.ToUpper()) || s.location.ToUpper().Contains(SearchString.ToUpper()) || s.job_role.ToUpper().Contains(SearchString.ToUpper()) || s.job_type.ToUpper().Contains(SearchString.ToUpper()));
        //            }

        //            switch (sortOrder)
        //            {
        //                case "job_title":
        //                    plist = plist.OrderByDescending(s => s.job_title);
        //                    break;

        //                case "location":
        //                    plist = plist.OrderBy(s => s.location);
        //                    break;

        //                case "job_role":
        //                    plist = plist.OrderByDescending(s => s.job_role);
        //                    break;

        //                case "job_type":
        //                    plist = plist.OrderByDescending(s => s.job_type);
        //                    break;

        //                default:
        //                    plist = plist.OrderBy(s => s.job_title);
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            Console.WriteLine(ex.Message);
        //        }
        //        //for sorting
        //        int pageSize = 4;
        //        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Jobs Load Successfully..!!";
        //        return View(await PaginatedList<JobPostViewModel>.CreateAsync(plist.AsNoTracking(), pageNumber ?? 1, pageSize));
        //    }
        //}

        //public IQueryable<JobPostViewModel> Addon(List<JobPostViewModel> newlist)
        //{
        //    foreach (var item in newlist)
        //    {
        //        item.EncryptionId = _Protector.Protect(item.ID.ToString());
        //    }
        //    return newlist.AsQueryable();
        //}

        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex", "Home");
            }
            if (User.IsInRole("Interviewer"))
            {
                return RedirectToAction("InterviewerIndex", "Home");
            }
            else
            {
                return View();
            }
        }

        public String GetEncryptedID(int Id)
        {
            return _Protector.Protect(Id.ToString());
        }

        ///<summary>
        ///This method fetches Home page for Admin
        ///</summary>
        ///<returns></returns>
        public async Task<IActionResult> AdminIndex()
        {

            //counter part
            AdminPanelViewModel collection = new AdminPanelViewModel();
            try
            {
                collection.ApplicationCount = _dbContext.Candidate.Where(x => x.emailConfirmed == true).Count();
                collection.ActiveApplicationCount = _dbContext.jobApplications.ToList().Where(x => x.status == status_Pending).Count();
                collection.InterviewerCount = _dbContext.Users.Count();
                collection.SelectedCount = _dbContext.jobApplications.Where(x => x.status == status_Complete).Count();

                //upcoming schedules scheduleviewmodels
                var upcoming_schedules = await _schedulesPage.GetAllSchedules();
                upcoming_schedules = upcoming_schedules.Where(x => x.status != reqValue);
                List<SchedulesViewModel> schedulelist = new List<SchedulesViewModel>();


                schedulelist = upcoming_schedules.ToList();
                //foreach (var item in upcoming_schedules.ToList())
                //{
                //    //getting interviewers names
                //    List<UserModel> listOfUser = getInterviewerNames(item.ID);
                //    item.InterviewerNames = listOfUser;

                //    //job role fetching
                //    item.jobRole = getJobRoleByCandidateId(item.candidateId);

                //    //fetching only pending schedules
                //    data = _dbContext.jobApplications.Where(x => x.candidateId == item.candidateId).FirstOrDefault();
                //    if (data.status == status_Pending)
                //    {
                //        schedulelist.Add(item);
                //    }
                //}
                collection.upcoming_schedules = schedulelist;



                //notification jobapplicationViewModels.
                var notifyJobApplications = await _jobApplicationPage.getJobApplications();
                collection.selected_application = notifyJobApplications.Where(x => x.status == status_Complete && x.notified == false).ToList();
                foreach (var item in collection.selected_application)
                {
                    item.candidateName = _dbContext.Candidate.Where(x => x.ID == item.candidateId).FirstOrDefault().name;
                    item.position = getPositionByCandidateId(item.candidateId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(collection);
        }

        ///<summary>
        ///This method fetches Home page for Interviewer
        ///</summary>
        ///<returns></returns>
        public async Task<IActionResult> InterviewerIndex()
        {
            //Counter Part
            InterviewerPanelViewModel collection = new InterviewerPanelViewModel();


            var pending_schedules = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));

            //filtering the schedules for getting only the incompleted schedules

            collection.pending_schedules = pending_schedules.Where(x => x.status != reqValue).ToList();
            collection.PendingScheduleCount = pending_schedules.Where(x => x.status != reqValue).ToList().Count();
            collection.CompletedScheduleCount = pending_schedules.Where(x => x.status == reqValue).Count();

            return View(collection);
        }


        ///<summary>
        ///This method is used for setting notification update to database
        ///</summary>
        ///<param name = "id" ></ param >
        ///< returns ></ returns >
        public async Task<IActionResult> NotificationDetails(int id)
        {
            JobApplicationViewModel jobApplication = new JobApplicationViewModel();
            try
            {
                jobApplication = await _jobApplicationPage.getJobApplicationById(id);
                CandidateViewModel candidateDetail = await _candidatePage.getCandidateById(jobApplication.candidateId);
                jobApplication.candidate = candidateDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(jobApplication);
        }

        ///<summary>
        ///SETS THE NOTIFIED FLAG OF SCHEDULE TO TRUE WHEN ADMIN CONFIRMS THE NOTIFICATION
        ///</summary>
        ///<param name = "id" ></ param >
        ///< returns ></ returns >
        public async Task<IActionResult> UpdateNotification(int id)
        {
            JobApplicationViewModel model = new JobApplicationViewModel();
            try
            {
                //getting job application
                model = await _jobApplicationPage.getJobApplicationById(id);
                //setting flag
                model.notified = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("UpdateNotificationPost", model);

        }
        public async Task<IActionResult> UpdateNotificationPost(JobApplicationViewModel model)
        {
            try
            {
                await _jobApplicationPage.UpdateJobApplication(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("AdminIndex", "Home", new { });
        }

        ///<summary>
        ///FETCHING JOB POSITION USING CANDIDATE ID FROM DATABASE
        ///</summary>
        ///<param name = "id" ></ param >
        ///< returns ></ returns >
        public string getPositionByCandidateId(int id)
        {
            var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == id).FirstOrDefault().job_Id;
            var position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
            return position;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetJobsList()
        {
            IQueryable<JobPostViewModel> plist = null;
            List<JobPostViewModel> list = new List<JobPostViewModel>();

            try
            {
                plist = await _jobPostPage.getJobPost();
                list = plist.ToList();

                foreach (JobPostViewModel obj in list)
                {
                    obj.EncryptedId = HttpUtility.UrlEncode(RSACSPSample.EncodeServerName((obj.ID).ToString()));
                }

                //TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Jobs Load Successfully..!!";
                return Json(new { data = list });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = plist });
            }
        }

        /// <summary>
        /// This method retrieves Application-Form which is used to apply for a particular job [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ApplyForm(string id) //jobpost id
        {

            CandidateViewModel model = new CandidateViewModel();
            try
            {
                model.jobpostID = Convert.ToInt32(RSACSPSample.DecodeServerName(id));

                model.jobName = _dbContext.JobPost.FirstOrDefault(x => x.ID == model.jobpostID).job_title;

                //fetching all the degrees for candidate to apply with.

                ViewBag.ListOfDegree = SelectionList.GetDegreeList();
                ViewBag.ReferenceSelect = SelectionList.GetReferenceTypeList();
                ViewBag.NoticePeriod = SelectionList.GetNoticePeriodTypeList();

            }
            catch (Exception ex)
            {
                model = new CandidateViewModel();
            }
            return View(model);
        }

        public JsonResult GetDept(int Id)
        {
            using (RecruitmentPortalDbContext _dbContext = BaseContext.GetDbContext())
            {
                List<Department> DepartmentList = new List<Department>();

                //Getting data from database Using EntityFramework Core
                DepartmentList = _dbContext.Department.Where(a => a.DegreeId == Id).ToList();
                DepartmentList = DepartmentList.Where(x => x.isActive == true).ToList();
                if (DepartmentList != null)
                {
                    //Inserting Select item in List
                    DepartmentList.Insert(0, new Department { ID = 0, dept_name = "Select Department" });

                }
                return Json(new SelectList(DepartmentList, "ID", "dept_name"));
            }
        }

        public List<string> noticePeriodList()
        {
            List<string> periodList = new List<string>();
            periodList.Add("Select");
            periodList.Add("Immediate");
            periodList.Add("15 Days");
            periodList.Add("30 Days");
            periodList.Add("60 Days");
            periodList.Add("90 Days");
            return periodList;
        }


        #endregion
    }
}
