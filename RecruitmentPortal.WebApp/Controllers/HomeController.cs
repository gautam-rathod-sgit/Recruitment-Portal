using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Security;
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

        //    await _jobPostPageservices.UpdateJobPost(jobPostModel);
        //    return RedirectToAction("SendOTPToMail", model);
        //}

        /// <summary>
        /// This method fetches Home Page
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="sortOrder"></param>
        /// <param name="pageNumber"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string SearchString, string sortOrder, int? pageNumber, string s)
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
                //////checking if vacancy of job completed or not
                //var vacancy = _dbContext.JobPost.FirstOrDefault(x => x.ID == newModel.job_Id).vacancy;
                //var count_post = _dbContext.JobPostCandidate.Where(x => x.job_Id == newModel.job_Id).Count();

                //if (vacancy <= count_post)
                //{
                //    var jobPostModel = await _jobPostPageservices.getJobPostById(newModel.job_Id);
                //    return RedirectToAction("DeactivateJobPost", new { model, jobPostModel });
                //}


                //for candidate conflict
                if (s != null)
                    ViewData["msg"] = s;

                ViewData["CurrentSort"] = sortOrder;
                ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_title" : "";
                ViewData["LocationSortParm"] = String.IsNullOrEmpty(sortOrder) ? "location" : "";
                ViewData["JobRoleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_role" : "";
                ViewData["JobTypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_type" : "";


                IQueryable<JobPostViewModel> plist = await _jobPostPage.getJobPost();
                plist = plist.Where(x => x.isActive == true);

                try
                {
                    if (!String.IsNullOrEmpty(SearchString))
                    {
                        plist = plist.Where(s => s.job_title.ToUpper().Contains(SearchString.ToUpper()) || s.location.ToUpper().Contains(SearchString.ToUpper()) || s.job_role.ToUpper().Contains(SearchString.ToUpper()) || s.job_type.ToUpper().Contains(SearchString.ToUpper()));
                    }

                    switch (sortOrder)
                    {
                        case "job_title":
                            plist = plist.OrderByDescending(s => s.job_title);
                            break;

                        case "location":
                            plist = plist.OrderBy(s => s.location);
                            break;

                        case "job_role":
                            plist = plist.OrderByDescending(s => s.job_role);
                            break;

                        case "job_type":
                            plist = plist.OrderByDescending(s => s.job_type);
                            break;

                        default:
                            plist = plist.OrderBy(s => s.job_title);
                            break;
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                //for sorting
                int pageSize = 4;
                
                return View(await PaginatedList<JobPostViewModel>.CreateAsync(plist.AsNoTracking(), pageNumber ?? 1, pageSize));
            }

        }
        //public IQueryable<JobPostViewModel> Addon(List<JobPostViewModel> newlist)
        //{
        //    foreach (var item in newlist)
        //    {
        //        item.EncryptionId = _Protector.Protect(item.ID.ToString());
        //    }
        //    return newlist.AsQueryable();
        //}
        //public String GetEncryptedID(int Id)
        //{
        //    return _Protector.Protect(Id.ToString());
        //}

        /// <summary>
        /// This method fetches Home page for Admin
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AdminIndex()
        {
            if (User.Identity.IsAuthenticated)
            {
                //counter part
                AdminPanelViewModel collection = new AdminPanelViewModel();
            try
            {
                    JobApplications data = new JobApplications();
                    collection.ApplicationCount = _dbContext.Candidate.Where(x => x.emailConfirmed == true).Count();
                    collection.ActiveApplicationCount = _dbContext.jobApplications.ToList().Where(x => x.status == status_Pending).Count();
                    collection.InterviewerCount = _dbContext.Users.Count();
                    collection.SelectedCount = _dbContext.jobApplications.Where(x => x.status == status_Complete).Count();

                    //upcoming schedules scheduleviewmodels
                    var upcoming_schedules = await _schedulesPage.GetAllSchedules();
                    upcoming_schedules = upcoming_schedules.Where(x => x.status != reqValue);
                    List<SchedulesViewModel> schedulelist = new List<SchedulesViewModel>();


                    //schedulelist = upcoming_schedules.ToList();
                    foreach(var item in upcoming_schedules.ToList())
                    {
                        //getting interviewers names
                        List<UserModel> listOfUser = getInterviewerNames(item.ID);
                        item.InterviewerNames = listOfUser;

                        //job role fetching
                        item.jobRole = getJobRoleByCandidateId(item.candidateId);

                        //fetching only pending schedules
                        data = _dbContext.jobApplications.Where(x => x.candidateId == item.candidateId).FirstOrDefault();
                        if (data.status == status_Pending)
                        {
                            schedulelist.Add(item);
                        }
                    }
                    collection.upcoming_schedules = schedulelist;



                    //notification jobapplicationViewModels.
                    var notifyJobApplications = await _jobApplicationPage.getJobApplications();
                    collection.selected_application = notifyJobApplications.Where(x => x.status == status_Complete && x.notified == false).ToList();
                    foreach (var item in collection.selected_application)
                    {
                        item.candidateName = _dbContext.Candidate.Where(x => x.ID == item.candidateId).FirstOrDefault().name;
                        item.position = getPositionByCandidateId(item.candidateId);
                        //job role fetching
                        item.job_Role = getJobRoleByCandidateId(item.candidateId);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(collection);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// This method fetches Home page for Interviewer
        /// </summary>
        /// <returns></returns>
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


        ///// <summary>
        ///// This method is used for setting notification update to database
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public async Task<IActionResult> NotificationDetails(string id)
        //{
        //    JobApplicationViewModel jobApplication = new JobApplicationViewModel();
        //    try
        //    {
        //        jobApplication = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
        //        CandidateViewModel candidateDetail = await _candidatePage.getCandidateById(jobApplication.candidateId);
        //        jobApplication.candidate = candidateDetail;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
           
        //    return View(jobApplication);
        //}

        /// <summary>
        /// SETS THE NOTIFIED FLAG OF SCHEDULE TO TRUE WHEN ADMIN CONFIRMS THE NOTIFICATION
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateNotification(string id)
        {

            JobApplicationViewModel model = new JobApplicationViewModel();
            try
            {
                //getting job application
                model = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
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
        //-----------------------------------------------------------------------------------------------

        /// <summary>
        /// FETCHING JOB POSITION USING CANDIDATE ID FROM DATABASE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getPositionByCandidateId(int id)
        {
            var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == id).FirstOrDefault().job_Id;
            var position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
            return position;
        }



        /// <summary>
        /// FETCHING JOB ROLE USING CANDIDATE ID FROM DATABASE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getJobRoleByCandidateId(int id)
        {
            int job_ID;
            string job_role = null;

            job_ID = _dbContext.JobPostCandidate.AsNoTracking().FirstOrDefault(x => x.candidate_Id == id).job_Id;
            job_role = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_role;

            return job_role;

        }

        /// <summary>
        /// fetching interviewer's names by Schedule ID with help of SchedulesUsers.
        /// </summary>
        /// <param name="Sid"></param>
        /// <returns></returns>
        public List<UserModel> getInterviewerNames(int Sid)
        {
            List<SchedulesUsers> allusers = new List<SchedulesUsers>();
            List<UserModel> interviewer_names = new List<UserModel>();
            try
            {
                allusers = _dbContext.SchedulesUsers.Where(x => x.scheduleId == Sid).ToList();
                foreach (var user in allusers)
                {
                    UserModel userModel = new UserModel();
                    userModel.Name = _dbContext.Users.Where(x => x.Id == user.UserId).FirstOrDefault().UserName;
                    userModel.Id = _dbContext.Users.Where(x => x.Id == user.UserId).FirstOrDefault().Id;
                    interviewer_names.Add(userModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return interviewer_names;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
