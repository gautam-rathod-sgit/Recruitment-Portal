using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

namespace RecruitmentPortal.WebApp.Controllers
{
    public class SchedulesController : Controller
    {
        #region private variables
        private readonly ISchedulesPage _schedulesPage;
        private readonly ISchedulesUsersPage _schedulesUsersPage;
        private readonly IWebHostEnvironment _environment;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;
        //for dropdown
        private readonly RecruitmentPortalDbContext _dbContext;
        #endregion

        #region Constructor
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
        #endregion

        #region public methods
        /// <summary>
        /// GET ACTION METHOD FOR SCHEDULE DETAILS INPUT USING FORM
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            ViewBag.RoundTypeList = SelectionList.GetRoundTypeList();
            ViewBag.StatusTypeList = SelectionList.GetStatusTypeList();
            SchedulesViewModel model = new SchedulesViewModel();
            List<ApplicationUser> allusers = new List<ApplicationUser>();
            allusers = (from element in _dbContext.Users select element).ToList();
            ViewBag.users = allusers;

            try
            {
                if (id != null)
                {
                    int decrypted_id = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
                    
                    //setting foreign key in schedule
                    model.candidateId = decrypted_id;
                    //getting job application ID
                    model.jobAppId = _dbContext.jobApplications.AsNoTracking().FirstOrDefault(x => x.candidateId == decrypted_id).ID;
                    //getting candidate name and position for readonly in view
                    model.candidate_name = _dbContext.Candidate.Where(x => x.ID == decrypted_id).FirstOrDefault().name;
                    var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == decrypted_id).FirstOrDefault().job_Id;
                    model.position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
                    model.EncryptedId = id;
                    model.EncryptedJobApplicationId = RSACSPSample.EncodeServerName((model.jobAppId).ToString());
                    //getting dropdown of AspNetUsers from DB
                }
            }
            catch (Exception ex)
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
        public async Task<IActionResult> IndexPost(SchedulesViewModel model, string submit)
        {
            List<Schedules> allschedules = new List<Schedules>();
            JobApplications jobAppModel = new JobApplications();
            Candidate candidateModel = new Candidate();
            SchedulesViewModel latestRecord = new SchedulesViewModel();
           // JobApplicationViewModel jobApplicationViewModel = new JobApplicationViewModel();
            try
            {
                //for getting the job application of candidate in current scenario 
                //candidateModel = _dbContext.Candidate.Where(x => x.ID == model.candidateId).FirstOrDefault();
                jobAppModel = _dbContext.jobApplications.Where(x => x.candidateId == model.candidateId).FirstOrDefault();

                //checking for already existing round 

                allschedules = _dbContext.Schedules.Where(x => x.candidateId == model.candidateId).ToList();
                foreach (var item in allschedules)
                {


                    if (item.round == model.round)
                    {
                        TempData["msg"] = model.round;
                        return RedirectToAction("Details", "JobApplication", new { id = RSACSPSample.EncodeServerName(jobAppModel.ID.ToString()), conflict = TempData["msg"] });
                    }
                }

                //checking if any schedule exists on same date and time with same interviewer
                var validSchedule = isScheduleValid(model.datetime, model.Multiinterviewer);
                if (validSchedule)
                {
                    return RedirectToAction("Details", "JobApplication", new { id = RSACSPSample.EncodeServerName(jobAppModel.ID.ToString()), time_conflict = true });
                }

                //for new schedule Proceed

                //converting enum values to int
                model.status = Convert.ToInt32(model.statusvalue);
                model.round = Convert.ToInt32(model.roundValue);

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
                        UserId = GetUserIdByName(item)
                    };
                    await _schedulesUsersPage.AddNewSchedulesUsers(newModel);
                }
                switch (submit)
                {
                    case "Save":
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Schedule updated Successfully.";
                        return RedirectToAction("Details", "JobApplication", new { id = model.EncryptedJobApplicationId });

                    case "Save & Continue":
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "Schedule updated Successfully.";
                        return RedirectToAction("Index", "Schedules", new { id = RSACSPSample.EncodeServerName(latestRecord.candidateId.ToString()) });
                        //return RedirectToAction("Index", "Schedules", new { id = RSACSPSample.EncodeServerName(latestRecord.ID.ToString())});
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Details", "JobApplication", new { id = RSACSPSample.EncodeServerName(jobAppModel.ID.ToString()) });
        }

        /// <summary>
        /// Actions returns true if any schedule exists for any specific interviewer in range of 15 minutes.
        /// </summary>
        /// <param name="dateValue"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool isScheduleValid(DateTime dateValue, List<string> names)
        {
            var scheduleList = _dbContext.Schedules.ToList();
            foreach (var item in scheduleList)
            {
                DateTime startdate = dateValue;
                DateTime enddate = item.datetime;
                var Totalminutes = (int)startdate.Subtract(enddate).TotalMinutes;

                if (Math.Abs(Totalminutes) <= 15 && Math.Abs(Totalminutes) >= 0)
                {
                    //checking if any any selected interviewer have any already round 

                    //getting interviewer names of schedule
                    List<UserModel> listOfUser = getInterviewerNames(item.ID);

                    //checking if same interviewer exist
                    foreach (var existing_data in listOfUser)
                    {
                        foreach (var new_data in names)
                        {
                            if (existing_data.Id == new_data)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
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
        public string GetUserIdByName(string name)
        {
            return _userManager.FindByNameAsync(name).Result.Id;
        }
        #endregion



    }
}
