using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Security;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobApplicationController : Controller
    {
        private string date_format = "dd-MM-yyyy";
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private string status_Complete = Enum.GetName(typeof(JobApplicationStatus), 2);
        private string status_Rejected = Enum.GetName(typeof(JobApplicationStatus), 3);
        private readonly IJobApplicationPage _jobApplicationPage;
        private readonly ISchedulesPage _schedulesPage;
        private readonly ICandidatePage _candidatePage;
        private readonly ISchedulesUsersPage _schedulesUsersPage;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly IDataProtector _Protector;

        public JobApplicationController(IJobApplicationPage jobApplicationPage,
            RecruitmentPortalDbContext dbContext,
            ISchedulesPage schedulesPage, IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings,
            ISchedulesUsersPage schedulesUsersPage,
            ICandidatePage candidatePage)
        {
            _jobApplicationPage = jobApplicationPage;
            _schedulesPage = schedulesPage;
            _schedulesUsersPage = schedulesUsersPage;
            _candidatePage = candidatePage;
            _dbContext = dbContext;
            _Protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.JobAppDetailIdRouteValue);
        }


        /// <summary>
        /// SHOWING ALL THE ACTIVE-JOB APPLICATION IN LIST FORM
        /// </summary>
        /// <param name="conflict"></param>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string conflict, string SearchString)
        {
            if (conflict != null)
            {
                ViewBag.msg = conflict;
            }
            try
            {
                var models = await _jobApplicationPage.getJobApplications();
                models = models.Where(x => x.status == status_Pending);
                List<JobApplicationViewModel> newlist = new List<JobApplicationViewModel>();
                newlist = models.ToList();
                foreach (var item in newlist)
                {
                    item.candidateName = getCandidateNameById(item.candidateId);
                    item.position = getPositionByCandidateId(item.candidateId);
                }
                models = newlist.AsQueryable();
                //Added search box test
                if (!String.IsNullOrEmpty(SearchString))
                {
                    models = models.Where(s => s.position.ToUpper().Contains(SearchString.ToUpper()));
                }
                return View(models);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
            
        }


        /// <summary>
        /// Adding new Active-Job Application for proceeding
        /// </summary>
        /// <param name="Cid"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddNewJobApplications(int Cid)
        {
            //checking if candidate job application already exists
            try
            {
                var isRepeating = _dbContext.jobApplications.Where(x => x.candidateId == Cid).Any();
                if (isRepeating)
                {
                    TempData["msg"] = getCandidateNameById(Cid);
                    return RedirectToAction("Index", "JobApplication", new { conflict = TempData["msg"] });
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            //create jobapplication
            JobApplicationViewModel model = new JobApplicationViewModel();
            model.candidateId = Cid;
            model.round = 0;
            model.status = status_Pending;
            model.notified = false;
            try
            {
                await _jobApplicationPage.AddJobApplications(model);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToAction("Index","JobApplication");
        }


        /// <summary>
        /// for details of Application status on details click
        /// </summary>
        /// <param name="id"></param>
        /// <param name="conflict"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id, int conflict)
        {
            if (conflict != 0)
                ViewData["msg"] = conflict;

            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            try
            {
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            
            //getting candidate schedules
            CandidateViewModel model = new CandidateViewModel();
            model = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);
            //pushing candidate schedules to custom scheudules in job applicationViewmodel for view.
            jobApplicationModel.Schedules = model.Schedules;

            //giving interviewer name list to each schedule in jobapplication 
            try
            {
                if (jobApplicationModel.Schedules != null)
                {
                    int i = 0;
                    foreach (var schedule in jobApplicationModel.Schedules)
                    {
                        List<UserModel> listOfUser = getInterviewerNames(schedule.ID);
                        jobApplicationModel.Schedules[i].InterviewerNames = listOfUser;
                        //setting statusName using status int from ENUM
                        jobApplicationModel.Schedules[i].statusName = Enum.GetName(typeof(StatusType), jobApplicationModel.Schedules[i].status);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //getting candidate name
            jobApplicationModel.candidateName = getCandidateNameById(jobApplicationModel.candidateId);
            //getting position
            jobApplicationModel.position = getPositionByCandidateId(jobApplicationModel.candidateId);

            //for encrypted id
            jobApplicationModel.EncryptionId = _Protector.Protect(jobApplicationModel.ID.ToString());
            return View(jobApplicationModel);
        }



        /// <summary>
        ///     updating Joining data/status of Active-JobApplication [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UpdateJobApplication(string encry_id, bool complete, bool rejected)
        {
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            int id = Convert.ToInt32(_Protector.Unprotect(encry_id));
            //getting the jobapplication
            try
            {
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(id);
                if (complete)
                {
                    jobApplicationModel.status = status_Complete;
                }
               if(rejected)
                {
                    jobApplicationModel.status = status_Rejected;
                    return RedirectToAction("UpdateJobApplicationPost", jobApplicationModel);
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return View(jobApplicationModel);
        }
        /// <summary>
        /// updating Joining data/status of Active-JobApplication [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateJobApplicationPost(JobApplicationViewModel model)
        {
            try
            {
                await _jobApplicationPage.UpdateJobApplication(model);
                if(model.status == status_Rejected)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("SelectedJobApplications");
        }
        /// <summary>
        /// Listing all the selected job application
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SelectedJobApplications(string SearchString)
        {
            IQueryable<JobApplicationViewModel> models = null;
            try
            {
                models = await _jobApplicationPage.getJobApplications();
                models = models.Where(x => x.status == status_Complete);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            

            //getting candidate name and job position with candidateId of JobApplication model
            List<JobApplicationViewModel> newlist = new List<JobApplicationViewModel>();
            newlist = models.ToList();
            foreach (var item in newlist)
            {
                item.candidateName = getCandidateNameById(item.candidateId);
                item.position = getPositionByCandidateId(item.candidateId);
                item.date = item.joining_date.ToString(date_format);
            }
            models = newlist.AsQueryable();

            //Added search box test
            if (!String.IsNullOrEmpty(SearchString))
            {
                models = models.Where(s => s.position.ToUpper().Contains(SearchString.ToUpper()));
            }
            return View(models);
        }

        //================================================================================================

        //schedule update or delete
        /// <summary>
        ///  FOR UPDATING SCHEDULE OF ANY ACTIVE-JOB APPLICATION WHILE CHECKING CONFLICT [GET]
        /// </summary>
        /// <param name="JobApplicationId"></param>
        /// <param name="scheduleId"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UpdateScheduleOfJobApplication(int JobApplicationId, int scheduleId, bool delete = false)
        {
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            CandidateViewModel model = new CandidateViewModel();
            //checking for deleting schedule action
            try
            {
                if (delete == true)
                {
                    await _schedulesPage.DeleteSchedule(scheduleId);
                    return RedirectToAction("Details", "JobApplication", new { id = JobApplicationId });
                }
                //for updating schedules
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(JobApplicationId);

                //getting candidate
                model = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //finding schedule to be edited
            SchedulesViewModel scheduleModel = new SchedulesViewModel();
            foreach (var item in model.Schedules)
            {
                if(item.ID == scheduleId)
                {
                    scheduleModel = item;
                }
            }
            
            //getting dropdown of AspNetUsers from DB
            var allusers = (from element in _dbContext.Users select element).ToList();
            ViewBag.users = allusers;

            return View(scheduleModel);
        }
        /// <summary>
        /// FOR UPDATING SCHEDULE OF ANY ACTIVE-JOB APPLICATION WHILE CHECKING CONFLICT [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateScheduleOfJobApplicationPost(SchedulesViewModel model)
        {
            List<SchedulesUsers> collection = new List<SchedulesUsers>();
            List<string> interviewers = new List<string>();
            JobApplications jobApplication = new JobApplications();
            try
            {
                //converting enum values to int
                model.status = Convert.ToInt32(model.statusvalue);

                //saving all the interviewers
                interviewers = model.Multiinterviewer;

                //all data of ScheduleUser composite table
                collection = _dbContext.SchedulesUsers.Where(x => x.scheduleId == model.ID).ToList();

                //removing the existing pairs of old schedules interviewers
                _dbContext.SchedulesUsers.RemoveRange(collection);
                _dbContext.SaveChanges();

                //adding new records of composite key into schedule users for new userschedule pairs
                foreach (var item in interviewers)
                {
                    SchedulesUsersViewModel newModel = new SchedulesUsersViewModel()
                    {
                        scheduleId = model.ID,
                        UserId = item
                    };
                    await _schedulesUsersPage.AddNewSchedulesUsers(newModel);
                }

                await _schedulesPage.UpdateSchedule(model);
                //redirecting to details of job Application
                jobApplication = _dbContext.jobApplications.Where(x => x.candidateId == model.candidateId).FirstOrDefault();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
            return RedirectToAction("Details", "JobApplication", new { id = jobApplication.ID });

        }

        //==================================================================================================================


        /// <summary>
        /// fetching interviewer's names by Schedule ID with help of SchedulesUsers.
        /// </summary>
        /// <param name="Sid"></param>
        /// <returns></returns>
        [HttpGet]
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
        /// FETCHING CANDIDATE NAME USING CANDIDATE ID FROM DATABASE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getCandidateNameById(int id)
        {
            string name = null;
            try
            {
                name =  _dbContext.Candidate.AsNoTracking().FirstOrDefault(x => x.ID == id).name;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return name;
        }

        /// <summary>
        /// FETCHING JOB POSITION USING CANDIDATE ID FROM DATABASE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getPositionByCandidateId(int id)
        {
            int job_ID;
            string position = null;
            try
            {
                job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == id).FirstOrDefault().job_Id;
                position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return position;
        }

    }
}
