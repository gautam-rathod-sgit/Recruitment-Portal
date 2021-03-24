using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Security;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class JobApplicationController : Controller
    {
        #region private variables
        private string date_format = "dd-MM-yyyy";
        string activeMode = "Active";
        private static bool goback = false;
        //jobApplicationStatus
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private string status_Complete = Enum.GetName(typeof(JobApplicationStatus), 2);
        private string status_Rejected = Enum.GetName(typeof(JobApplicationStatus), 3);

        //Application mode
        private string mode_All = Enum.GetName(typeof(ApplicationMode), 1);
        private string mode_Pending = Enum.GetName(typeof(ApplicationMode), 2);
        private string mode_Active = Enum.GetName(typeof(ApplicationMode), 3);
        private string mode_Selected = Enum.GetName(typeof(ApplicationMode), 4);
        private string mode_Rejected = Enum.GetName(typeof(ApplicationMode), 5);

        private readonly IJobApplicationPage _jobApplicationPage;
        private readonly ISchedulesPage _schedulesPage;
        private readonly ICandidatePage _candidatePage;
        private readonly ISchedulesUsersPage _schedulesUsersPage;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly IDataProtector _Protector;
        #endregion

        #region Constructor
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
        #endregion

        #region Public Methods

        /// <summary>
        /// For Showing list of Active Applications
        /// </summary>
        /// <param name="conflict"></param>
        /// <param name="SearchString"></param>
        /// <param name="Application_mode"></param>
        /// <param name="istatus"></param>
        /// <returns></returns>
        //----riddhi----

        //public async Task<IActionResult> Index(string conflict, string SearchString, string Application_mode, string istatus = "Scheduled")
        //{
        //    if (conflict != null)
        //    {
        //        ViewBag.msg = conflict;
        //    }
        //    try
        //    {
        //        List<Schedules> mylist = new List<Schedules>();
        //        //creating a select list for selecting status type applications
        //        List<SelectListItem> ObjItem = new List<SelectListItem>()
        //        {
        //        new SelectListItem {Text="All Applications",Value = mode_All},
        //        new SelectListItem {Text="New",Value = mode_Pending},
        //        new SelectListItem {Text="Active",Value = mode_Active },
        //        new SelectListItem {Text="Selected",Value = mode_Selected},
        //        new SelectListItem {Text="Rejected",Value = mode_Rejected},
        //        };
        //        ViewBag.menuSelect = ObjItem;

        //        //creating a select list for selecting Interview Status
        //        List<SelectListItem> ObjStatus = new List<SelectListItem>()
        //        {
        //        new SelectListItem {Text="Scheduled",Value="Scheduled"},
        //        new SelectListItem {Text="Pending",Value="Pending"},
        //        new SelectListItem {Text="Completed",Value="Completed"},
        //        };
        //        ViewBag.statusSelect = ObjStatus;

        //        var models = await _jobApplicationPage.getJobApplications();
        //        models = models.Where(x => x.status == status_Pending);
        //        List<JobApplicationViewModel> newlist = new List<JobApplicationViewModel>();
        //        newlist = models.ToList();
        //        foreach (var item in newlist)
        //        {
        //            item.candidateName = getCandidateNameById(item.candidateId);
        //            item.position = getPositionByCandidateId(item.candidateId);
        //            item.job_Role = getJobRoleByCandidateId(item.candidateId);
        //            item.interview_Status = getInterviewStatusByCandidateId(item.candidateId);
        //        }

        //        newlist = newlist.Where(x => x.interview_Status == istatus).ToList();
        //        models = newlist.AsQueryable();

        //        return View(models);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return View();

        //}

        
        
        public IActionResult Index()
        {
            ViewBag.DegreeList = SelectionList.GetDegreeList().Select(m => new { Id = m.ID, Name = m.degree_name });

            return View();
        }

        
        /// <summary>
        /// Adding new Active-Job Application for proceeding
        /// </summary>
        /// <param name="Cid"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddNewJobApplications(string Cid)
        {
            int decrypted_key = Convert.ToInt32(RSACSPSample.DecodeServerName(Cid));
            //checking if candidate job application already exists
            try
            {

                var isRepeating = _dbContext.jobApplications.Where(x => x.candidateId == decrypted_key).Any();
                if (isRepeating)
                {
                    TempData["msg"] = getCandidateNameById(decrypted_key);
                    return RedirectToAction("AllApplications", "Candidate", new { Application_mode = status_Pending, conflict = TempData["msg"] });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //create jobapplication
            JobApplicationViewModel model = new JobApplicationViewModel();
            model.candidateId = decrypted_key;
            model.round = 0;
            model.status = status_Pending;
            model.notified = false;
            model.start_date = DateTime.Now;
            try
            {
                await _jobApplicationPage.AddJobApplications(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("AllApplications", "Candidate", new { Application_mode = "Active" });
        }





        /// <summary>
        /// for details of Application status on details click
        /// </summary>
        /// <param name="id"></param>
        /// <param name="conflict"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            int actualId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            jobApplicationModel = await _jobApplicationPage.getJobApplicationById(actualId);
            jobApplicationModel.EncryptedCandidateId = RSACSPSample.EncodeServerName(jobApplicationModel.candidateId.ToString());
            jobApplicationModel.EncryptedJobId = id;
            jobApplicationModel.candidateName = getCandidateNameById(jobApplicationModel.candidateId);
            jobApplicationModel.position = getPositionByCandidateId(jobApplicationModel.candidateId);
            jobApplicationModel.job_Role = getJobRoleByCandidateId(jobApplicationModel.candidateId);
           
              jobApplicationModel.candidate = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);

            return View(jobApplicationModel);
        }

        [HttpPost]
        public async Task<IActionResult> Details(JobApplicationViewModel model)
        {
            //JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            //int actualId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            //jobApplicationModel = await _jobApplicationPage.getJobApplicationById(actualId);
            //jobApplicationModel.EncryptedCandidateId = RSACSPSample.EncodeServerName(jobApplicationModel.candidateId.ToString());
            //jobApplicationModel.EncryptedJobId = id;
            //jobApplicationModel.candidateName = getCandidateNameById(jobApplicationModel.candidateId);
            //jobApplicationModel.position = getPositionByCandidateId(jobApplicationModel.candidateId);
            //jobApplicationModel.job_Role = getJobRoleByCandidateId(jobApplicationModel.candidateId);

            //jobApplicationModel.candidate = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);

            return View(model);
        }

        public async Task<IActionResult> GetActiveAppDetailsList(string id)
        {
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            CandidateViewModel model = new CandidateViewModel();
            try
            {
                if (id != null)
                {
                    jobApplicationModel = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                }

                if (jobApplicationModel != null && id != null)
                {
                    //getting candidate schedules
                    model = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);

                    //pushing candidate schedules to custom scheudules in job applicationViewmodel for view.

                    jobApplicationModel.Schedules = model.Schedules;


                    //giving interviewer name list to each schedule in jobapplication 
                    if (jobApplicationModel.Schedules.Count > 0)
                    {
                        foreach (var schedule in jobApplicationModel.Schedules)
                        {
                            List<UserModel> listOfUser = getInterviewerNames(schedule.ID);
                            schedule.InterviewerNames = listOfUser;
                            schedule.statusName = Enum.GetName(typeof(StatusType), schedule.status);
                        }
                    }
                    //getting candidate name
                    jobApplicationModel.candidateName = getCandidateNameById(jobApplicationModel.candidateId);
                    //getting position
                    jobApplicationModel.position = getPositionByCandidateId(jobApplicationModel.candidateId);

                    //----------riddhi--
                    //getting job-role
                    jobApplicationModel.job_Role = getJobRoleByCandidateId(jobApplicationModel.candidateId);
                    //getting applied-date
                    jobApplicationModel.date = model.apply_date;
                    jobApplicationModel.candidate = model;
                    jobApplicationModel.joining_date = DateTime.Now;
                }

                return Json(new { data = jobApplicationModel.Schedules });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = jobApplicationModel.Schedules });
            }

        }


        /// <summary>
        /// Updating Joining data/status of Active-JobApplication [GET] BUT IT'S NOT USED DEU TO MODAL POPUP FORM
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UpdateJobApplication(string id, bool complete, bool rejected)
        {
            DateTime thisDay = DateTime.Today;
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();

            //getting the jobapplication
            try
            {
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                if (complete)
                {
                    jobApplicationModel.status = status_Complete;
                    jobApplicationModel.joining_date = DateTime.Parse(thisDay.ToString("d"));
                }
                if (rejected)
                {
                    jobApplicationModel.status = status_Rejected;
                    jobApplicationModel.flag_Rejected = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // return View(jobApplicationModel);
             return RedirectToAction("UpdateJobApplicationPost", "JobApplication", jobApplicationModel);
          //  return RedirectToAction("SendRejectionMailToCandidate", "Candidate", new { id = jobApplicationModel.candidateId });
        }
        /// <summary>
        /// updating Joining data/status of Active-JobApplication [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateJobApplicationPost(JobApplicationViewModel model, bool editFromMenu)
        {
            try
            {
                if (model.flag_Rejected)
                {
                    model.status = status_Rejected;
                    model.rejection_date = DateTime.Now;
                    await _jobApplicationPage.UpdateJobApplication(model);
                    return RedirectToAction("SendRejectionMailToCandidate", "Candidate", new { id = model.candidateId });
                }

                if (!model.flag_Edit)
                {
                    model.status = model.flag_Accepted ? status_Complete : status_Pending;
                    model.accept_date = DateTime.Now;
                    await _jobApplicationPage.UpdateJobApplication(model);
                }
                else
                {
                    await _jobApplicationPage.UpdateJobApplication(model);
                }

                if (model.flag_Edit)
                {
                    if (!model.editFromMenu)
                    {
                        return RedirectToAction("SelectedJobApplicationsDetails", "JobApplication", new { id = RSACSPSample.EncodeServerName((model.ID).ToString()) });

                    }
                    else
                    {
                        return RedirectToAction("SelectedJobApplicationsDetails", "JobApplication", new { id = RSACSPSample.EncodeServerName((model.ID).ToString()), toAdminIndex = true });

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToAction("AllApplications", "Candidate", new { Application_mode = status_Complete });
        }
       

        /// <summary>
        /// For displaying details of selected candidate application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SelectedJobApplicationsDetails(string id, bool toAdminIndex = false)
        {  
           
            JobApplicationViewModel model = new JobApplicationViewModel();
            try
            {
                if (toAdminIndex)
                {
                    ViewBag.toAdminIndex = toAdminIndex;
                    toAdminIndex = false;
                }
                model = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                model.position = getPositionByCandidateId(model.candidateId);
                model.job_Role = getJobRoleByCandidateId(model.candidateId);
                model.interview_Status = _jobApplicationPage.getInterviewStatusForApplication(model.candidateId);
                model.candidate = await _candidatePage.getCandidateById(model.candidateId);

                //if(model.status == status_Rejected)
                //{
                model.listOfRounds = getListOfRounds(model.candidateId);
                foreach(var item in model.listOfRounds)
                {
                    model.allRounds += item + ",";
                }
                
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(model);
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
        public async Task<IActionResult> UpdateScheduleOfJobApplication(string id, string scheduleId, bool to_schedule, bool remove = false)
        {
            if (to_schedule)
            {
                ViewBag.goback = to_schedule;
            }
            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            CandidateViewModel model = new CandidateViewModel();
            int JobApplicationId = 0;
            if (id != null)
            {
                JobApplicationId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            }
            int schedule_ID = Convert.ToInt32(RSACSPSample.DecodeServerName((scheduleId)));
            //checking for deleting schedule action
            try
            {
                if (remove == true)
                {
                    await _schedulesPage.DeleteSchedule(schedule_ID);
                    return RedirectToAction("Index", "Interviewer");
                }
                //for updating schedules
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(JobApplicationId);

                //getting candidate
                model = await _candidatePage.getCandidateByIdWithSchedules(jobApplicationModel.candidateId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //finding schedule to be edited
            SchedulesViewModel scheduleModel = new SchedulesViewModel();
            foreach (var item in model.Schedules)
            {
                if (item.ID == schedule_ID)
                {
                    scheduleModel = item;
                }
            }
            //assignning jobappID to schedule for coming back to details page after update/delete
            scheduleModel.jobAppId = JobApplicationId;
            scheduleModel.roundName = Enum.GetName(typeof(RoundType), scheduleModel.round);
            scheduleModel.roundValue = (RoundType)Enum.Parse(typeof(RoundType), scheduleModel.roundName);

            //getting dropdown of AspNetUsers from DB
            var allusers = (from element in _dbContext.Users select element).ToList();

            ViewBag.RoundTypeList = SelectionList.GetRoundTypeList();
            ViewBag.StatusTypeList = SelectionList.GetStatusTypeList();
            ViewBag.users = allusers;

            scheduleModel.EncryptedId = RSACSPSample.EncodeServerName((scheduleModel.ID).ToString());

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
                model.round = Convert.ToInt32(model.roundValue);

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
                    //getting userId for username
                    var fetchedUserId = getUserIdByUsername(item);

                    SchedulesUsersViewModel newModel = new SchedulesUsersViewModel()
                    {
                        scheduleId = model.ID,
                        UserId = fetchedUserId
                    };
                    await _schedulesUsersPage.AddNewSchedulesUsers(newModel);
                }

                await _schedulesPage.UpdateSchedule(model);
                //redirecting to details of job Application
                jobApplication = _dbContext.jobApplications.Where(x => x.candidateId == model.candidateId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

                return RedirectToAction("Details", "JobApplication", new { id = RSACSPSample.EncodeServerName((jobApplication.ID).ToString()) });

        }

        /// <summary>
        /// Confirming the selected job application notification by setting notified flag.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmJobApplication(string id)
        {

            JobApplicationViewModel jobApplicationModel = new JobApplicationViewModel();
            //getting the jobapplication
            try
            {
                jobApplicationModel = await _jobApplicationPage.getJobApplicationById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                jobApplicationModel.notified = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("ConfirmJobApplicationPost", jobApplicationModel);
        }
        /// <summary>
        /// Post method for updating the confirmation into database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmJobApplicationPost(JobApplicationViewModel model)
        {
            await _jobApplicationPage.UpdateJobApplication(model);
            return RedirectToAction("SelectedJobApplicationsDetails", "JobApplication", new { id = RSACSPSample.EncodeServerName((model.ID).ToString()) });
        }

        /// <summary>
        /// For Reopening the job application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ReOpenJobApplication(string id)
        {

            //setting the status to pending
            var model = _dbContext.jobApplications.Where(x => x.ID == Convert.ToInt32(RSACSPSample.DecodeServerName(id))).FirstOrDefault();
            model.status = status_Pending;
            model.start_date = DateTime.Now;

            //removing the existing schedules of that candidate.

            //all data of ScheduleUser composite table
            var collection = _dbContext.Schedules.Where(x => x.candidateId == model.candidateId).ToList();

            //removing the existing pairs of old schedules of candidate
            _dbContext.Schedules.RemoveRange(collection);
            _dbContext.SaveChanges();

            return RedirectToAction("AllApplications", "Candidate", new { Application_mode = activeMode });
        }



        //==================================================================================================================


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


        /// <summary>
        /// getting userId using username from database
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string getUserIdByUsername(string username)
        {
            var allusers = (from element in _dbContext.Users select element).ToList();
            foreach (var item in allusers)
            {
                if(item.UserName == username)
                return item.Id;
            }
            return null;
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
                name = _dbContext.Candidate.AsNoTracking().FirstOrDefault(x => x.ID == id).name;

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
                job_ID = _dbContext.JobPostCandidate.AsNoTracking().FirstOrDefault(x => x.candidate_Id == id).job_Id;
                position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
        /// Gives the named value list of a job application schedules
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> getListOfRounds(int id)
        {
            List<string> list = new List<string>();
            var data = _dbContext.Schedules.Where(x => x.candidateId == id).ToList();
            foreach (var item in data)
            {
                list.Add(Enum.GetName(typeof(RoundType), item.round));
            }
            return list;
        }

       

        
        #endregion
    }
}
