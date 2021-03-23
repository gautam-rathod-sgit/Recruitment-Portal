using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class InterviewerController : BaseController
    {
        private string time_format = "HH:mm tt";
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private int status_scheduled = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Scheduled"));
        private int status_pending = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Pending"));
        private int status_completed = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Completed"));
        private IQueryable<SchedulesViewModel> slist;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly ISchedulesPage _schedulesPage;
        private readonly ICandidatePage _candidatePageServices;
        private readonly UserManager<ApplicationUser> _userManager;
        public InterviewerController(UserManager<ApplicationUser> userManager, ICandidatePage candidatePage, RecruitmentPortalDbContext dbContext, ISchedulesPage schedulesPage)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _schedulesPage = schedulesPage;
            _candidatePageServices = candidatePage;
        }


        /// <summary>
        /// SHOWING ALL THE SCHEDULES FOR INTERVIEWER 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// method for getting all the Schedules of Interviewer.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                JobApplications allSchedules = new JobApplications();
                List<SchedulesViewModel> schedulelist = new List<SchedulesViewModel>();
                List<SchedulesViewModel> viewScheduleList = new List<SchedulesViewModel>();

                if (User.IsInRole("Admin"))
                {
                    //getting all the schedules of all candidates
                    slist = await _schedulesPage.GetAllSchedules();

                    //filtering the schedules for getting only the incompleted schedules (for active applications only)
                    slist = slist.Where(x => x.status == status_scheduled || x.status == status_pending);
                    viewScheduleList = slist.ToList();
                    //schedulelist = upcoming_schedules.ToList();
                    foreach (var item in viewScheduleList)
                    {
                        allSchedules = _dbContext.jobApplications.Where(x => x.candidateId == item.candidateId).FirstOrDefault();

                        //getting job application ID
                        item.jobAppId = _dbContext.jobApplications.AsNoTracking().FirstOrDefault(x => x.candidateId == item.candidateId).ID;
                        item.jobRole = getJobRoleByCandidateId(item.candidateId);
                        item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                        item.roundName = Enum.GetName(typeof(RoundType), item.round);
                        //getting interviewer names of schedule
                        List<UserModel> listOfUser = getInterviewerNames(item.ID);
                        item.InterviewerNames = listOfUser;

                        //creating new schedule list 
                        if (allSchedules.status == status_Pending)
                        {
                            schedulelist.Add(item);
                        }
                    }
                    //assigning to view passing list
                    slist = schedulelist.AsQueryable();
                }
                else
                {
                    slist = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));
                    //filtering the schedules for getting only the completed schedules for interviewer
                    slist = slist.Where(x => x.status == status_completed);
                    foreach (var item in slist)
                    {
                        item.jobRole = getJobRoleByCandidateId(item.candidateId);
                        item.roundName = Enum.GetName(typeof(RoundType), item.round);
                        item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                        schedulelist.Add(item);
                    }
                    slist = schedulelist.AsQueryable();
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = slist });
            }
            return Json(new { data = slist });
        }

        /// <summary>
        /// gives the JobRole using candidate id for which that candidate has applied.
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
        /// enlist all the interviewer names of particular schedule using schedule Id
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

        /// <summary>
        /// ENLIST ALL THE DETAILS OF PARTICULAR SCHEDULE FOR CANDIDATE TO INTERVIEWER
        /// </summary>
        /// <param name="Sid"></param>
        /// <returns></returns>
        public async Task<IActionResult> ScheduleDetails(string scheduleId, bool toAdminIndex, bool toCompletedList)
        {
            if (toAdminIndex)
            {
                ViewBag.toAdminIndex = toAdminIndex;
            }

            //getting schedule by ID
            SchedulesViewModel model = await _schedulesPage.GetSchedulesById(Convert.ToInt32(RSACSPSample.DecodeServerName(scheduleId)));
            //getting job application ID
            model.jobAppId = _dbContext.jobApplications.AsNoTracking().FirstOrDefault(x => x.candidateId == model.candidateId).ID;
            model.candidate = await _candidatePageServices.getCandidateById(model.candidateId);
            model.candidate.jobRole = getJobRoleByCandidateId(model.candidate.ID);
            try
            {
                //assigning values to some fields
                model.time = model.datetime.ToString(time_format);
                model.statusName = Enum.GetName(typeof(StatusType), model.status);
                model.roundName = Enum.GetName(typeof(RoundType), model.round);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return View(model);
        }

        //update schedule
        /// <summary>
        /// ACTION WILL UPDATE THE STATUS OF SCHEDULE WITH REMARKS, STATUS, RATING GIVEN BY INTERVIEWER.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Update_Schedule_GET(string id)
        {

            SchedulesViewModel model = new SchedulesViewModel();
            try
            {
                model = await _schedulesPage.GetSchedulesById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update_Schedule_POST(SchedulesViewModel model)
        {
            try
            {
                //converting enum values to int
                model.status = Convert.ToInt32(model.statusvalue);

                await _schedulesPage.UpdateSchedule(model);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("ScheduleDetails", new { scheduleId = RSACSPSample.EncodeServerName((model.ID).ToString()) });
        }
    }
}
