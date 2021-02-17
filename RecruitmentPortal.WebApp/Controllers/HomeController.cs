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
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        public HomeController(ICandidatePage candidatePage,
            IJobApplicationPage jobApplicationPage,
            ISchedulesPage schedulesPage,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            RecruitmentPortalDbContext dbContext,
            IJobPostPage jobPostPage)
        {
            _logger = logger;
            _dbContext = dbContext;
            _jobApplicationPage = jobApplicationPage;
            _schedulesPage = schedulesPage;
            _candidatePage = candidatePage;
            _userManager = userManager;
            _jobPostPage = jobPostPage;
        }

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
                return RedirectToAction("Index", "Interviewer");
            }
            else
            {
                //for candidate conflict
                if (s != null)
                    ViewData["msg"] = s;

                ViewData["CurrentSort"] = sortOrder;
                ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_title" : "";
                ViewData["LocationSortParm"] = String.IsNullOrEmpty(sortOrder) ? "location" : "";
                ViewData["JobRoleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_role" : "";
                ViewData["JobTypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_type" : "";


                IQueryable<JobPostViewModel> plist = await _jobPostPage.getJobPost();

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

        /// <summary>
        /// This method fetches Home page for Admin
        /// </summary>
        /// <returns></returns>
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
                var upcoming_schedules = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));
                collection.upcoming_schedules = upcoming_schedules.Where(x => x.status != reqValue).ToList();

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

        //setting notification update to database
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// This method is used for setting notification update to database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// SETS THE NOTIFIED FLAG OF SCHEDULE TO TRUE WHEN ADMIN CONFIRMS THE NOTIFICATION
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
