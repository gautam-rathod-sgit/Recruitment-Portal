using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class SchedulesController : Controller
    {

        private readonly ISchedulesPage _schedulesPage;
        private readonly ISchedulesUsersPage _schedulesUsersPage;

        private readonly IWebHostEnvironment _environment;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;
        //for dropdown
        private readonly RecruitmentPortalDbContext _dbContext;
        public SchedulesController(ISchedulesPage schedulesPage,
             ISchedulesUsersPage schedulesUsersPage,
            IWebHostEnvironment environment,
             UserManager<ApplicationUser> userManager,
            RecruitmentPortalDbContext dbContext)
        {
            _schedulesPage = schedulesPage;
            _dbContext = dbContext;
            _environment = environment;
            _userManager = userManager;
            _schedulesUsersPage = schedulesUsersPage;
        } 


        /// <summary>
        /// GET ACTION METHOD FOR SCHEDULE DETAILS INPUT USING FORM
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            SchedulesViewModel model = new SchedulesViewModel();
            List<ApplicationUser> allusers = new List<ApplicationUser>();
            try
            {
                //setting foreign key in schedule
                model.candidateId = id;

                //getting candidate name and position for readonly in view
                model.candidate_name = _dbContext.Candidate.Where(x => x.ID == id).FirstOrDefault().name;
                var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == id).FirstOrDefault().job_Id;
                model.position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;

                //getting dropdown of AspNetUsers from DB
                allusers = (from element in _dbContext.Users select element).ToList();
                ViewBag.users = allusers;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(model);
        }

        /// <summary>
        /// POST ACTION METHOD FOR SCHEDULE DETAILS INPUT INTO DATABASE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndexPost(SchedulesViewModel model)
        {
            List<Schedules> allschedules = new List<Schedules>();
            JobApplications jobAppModel = new JobApplications();
            Candidate candidateModel = new Candidate();
            SchedulesViewModel latestRecord = new SchedulesViewModel();

            try
            {
                //for getting the job application of candidate in current scenario (we are in schedules so for getting 
                //job Application ID, need to go through candidate)
                candidateModel = _dbContext.Candidate.Where(x => x.ID == model.candidateId).FirstOrDefault();
                jobAppModel = _dbContext.jobApplications.Where(x => x.candidateId == model.candidateId).FirstOrDefault();

                //checking for already existing round
                allschedules = _dbContext.Schedules.Where(x => x.candidateId == model.candidateId).ToList();
                foreach(var item in allschedules)
                {
                    if(item.round == model.round)
                    {
                        TempData["msg"] = model.round;
                        return RedirectToAction("Details", "JobApplication", new { id = jobAppModel.ID , conflict = TempData["msg"] });
                    }
                }
                //for new schedule Proceed
                //converting enum values to int
                model.status = Convert.ToInt32(model.statusvalue);

                //saving all the interviewers
                List<string> interviewers = new List<string>();
                interviewers = model.Multiinterviewer;

                //fetch schedule from db for this candidate , getting schedule id for composite key use.
                latestRecord = await _schedulesPage.AddNewSchedules(model);

                //insert Composite keys to SchedulesUsers (interviewers to schedules mapping)
                foreach (var item in interviewers)
                {
                    SchedulesUsersViewModel newModel = new SchedulesUsersViewModel()
                    {
                        scheduleId = latestRecord.ID,
                        UserId = item
                    };
                    await _schedulesUsersPage.AddNewSchedulesUsers(newModel);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Details", "JobApplication", new { id = jobAppModel.ID });
        }
    }
}
