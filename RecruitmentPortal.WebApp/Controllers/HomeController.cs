using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.Infrastructure.Repository;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Models;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.Security;
using RecruitmentPortal.WebApp.Services;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        IQueryable<SchedulesViewModel> allScheduleList;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly IJobPostPage _jobPostPage;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISchedulesPage _schedulesPage;
        private readonly IJobPostCandidatePage _jobPostCandidatePage;
        private readonly ICandidatePage _candidatePageServices;
        private readonly IDegreePage _degreePageServices;
        private readonly IDepartmentPage _departmentPageservices;
        private readonly IJobPostPage _jobPostPageservices;
        private readonly IJobApplicationPage _jobApplicationPage;

        private readonly IDataProtector _Protector;
        public IEmailService _emailService { get; }
        private readonly IWebHostEnvironment _environment;


        #endregion

        #region Constructor
        public HomeController(ICandidatePage candidatePage,
            IWebHostEnvironment environment,
            IJobApplicationPage jobApplicationPage,
            ISchedulesPage schedulesPage,
            UserManager<ApplicationUser> userManager,
                         IDepartmentPage departmentPageservices,

              IJobPostCandidatePage jobPostCandidatePage,
            ILogger<HomeController> logger,
                     IDegreePage degreePageServices,
            RecruitmentPortalDbContext dbContext,
       
            IJobPostPage jobPostPageservices,

            IDataProtectionProvider dataProtectionProvider,
             IEmailService emailService,
            DataProtectionPurposeStrings dataProtectionPurposeStrings,
            IJobPostPage jobPostPage)
        {
            _logger = logger;
            _departmentPageservices = departmentPageservices;
            _environment = environment;
            _emailService = emailService;
            _jobPostPageservices = jobPostPageservices;

            _dbContext = dbContext;
            _jobApplicationPage = jobApplicationPage;
            _schedulesPage = schedulesPage;
            _candidatePageServices = candidatePage;
            _userManager = userManager;
            _degreePageServices = degreePageServices;

            _jobPostCandidatePage = jobPostCandidatePage;
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

        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex", "Account");
            }
            if (User.IsInRole("Interviewer"))
            {
                return RedirectToAction("InterviewerIndex", "Account");
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



        /////<summary>
        /////This method is used for setting notification update to database
        /////</summary>
        /////<param name = "id" ></ param >
        /////< returns ></ returns >
        //public async Task<IActionResult> NotificationDetails(int id)
        //{
        //    JobApplicationViewModel jobApplication = new JobApplicationViewModel();
        //    try
        //    {
        //        jobApplication = await _jobApplicationPage.getJobApplicationById(id);
        //        CandidateViewModel candidateDetail = await _candidatePageServices.getCandidateById(jobApplication.candidateId);
        //        jobApplication.candidate = candidateDetail;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return View(jobApplication);
        //}

        /////<summary>
        /////SETS THE NOTIFIED FLAG OF SCHEDULE TO TRUE WHEN ADMIN CONFIRMS THE NOTIFICATION
        /////</summary>
        /////<param name = "id" ></ param >
        /////< returns ></ returns >
        //public async Task<IActionResult> UpdateNotification(int id)
        //{
        //    JobApplicationViewModel model = new JobApplicationViewModel();
        //    try
        //    {
        //        //getting job application
        //        model = await _jobApplicationPage.getJobApplicationById(id);
        //        //setting flag
        //        model.notified = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return RedirectToAction("UpdateNotificationPost", model);

        //}
        //public async Task<IActionResult> UpdateNotificationPost(JobApplicationViewModel model)
        //{
        //    try
        //    {
        //        await _jobApplicationPage.UpdateJobApplication(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return RedirectToAction("AdminIndex", "Home", new { });
        //}

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetJoballScheduleList()
        {
            IQueryable<JobPostViewModel> plist = null;
            List<JobPostViewModel> list = new List<JobPostViewModel>();

            try
            {
                plist = await _jobPostPage.getJobPost();
                plist = plist.Where(x => x.isActive == true);
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

                model.jobpostName = _dbContext.JobPost.FirstOrDefault(x => x.ID == model.jobpostID).job_title;

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

        /// <summary>
        /// This method stores the applicant's form-data to the database [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ApplyFormPost(CandidateViewModel model)
        {
            //checking if already applied for same job
            var candidateList = await _candidatePageServices.getCandidates();

            try
            {
                List<CandidateViewModel> list = new List<CandidateViewModel>();
                list = candidateList.ToList();
                foreach (var item in list)
                {
                    if (item.email == model.email)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = model.email + " already exists !!";
                        return RedirectToAction("ApplyForm", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            try
            {
                //------------------------------------------------------------------------------------
                //receiving dropdown value of degree
                if (model.degree == "Select Degree")
                {
                    ModelState.AddModelError("", "Select Degree");
                }
                //getting data from database
                List<Degree> DegreeList = new List<Degree>();
                DegreeList = (from element in _dbContext.Degree select element).ToList();
                DegreeList = DegreeList.Where(x => x.isActive == true).ToList();
                //inserting into dropdown list
                DegreeList.Insert(0, new Degree { ID = 0, degree_name = "Select Degree" });

                //assigning degreelist to viewbag.listofdegree
                ViewBag.ListOfDegree = DegreeList;
                //------------------------------------------------------------------------------------


                //getting selected value for degree
                DegreeViewModel selectedDegree = await _degreePageServices.getDegreeById(model.selectedDegree);
                string degreename = selectedDegree.degree_name;

                if (model.selectDept != 0)
                {
                    DepartmentViewModel selectedDept = await _departmentPageservices.getDepartmentById(model.selectDept);
                    string deptename = selectedDept.dept_name;

                    model.degree = degreename + "(" + deptename + ")";
                }
                else
                {
                    model.degree = degreename;
                }


                //assigning today's apply date 
                model.apply_date = Convert.ToDateTime(DateTime.Now.ToString("g"));

                //giving appying through
                model.applying_through = Enum.GetName(typeof(ReferenceType), Convert.ToInt32(model.applying_through));



                //Resume details fetching
                //create a place in wwwroot for storing uploaded images
                var uploads = Path.Combine(_environment.WebRootPath, "Resume");
                if (model.File != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, model.File.FileName), FileMode.Create))
                    {
                        await model.File.CopyToAsync(fileStream);
                    }
                    model.resume = model.File.FileName;

                }

                model.emailConfirmed = false;
                var latestRecord = await _candidatePageServices.AddNewCandidate(model);

                //add to jobpostcandidate
                JobPostCandidateViewModel newModel = new JobPostCandidateViewModel()
                {
                    job_Id = model.jobpostID,
                    candidate_Id = latestRecord.ID
                };
                await _jobPostCandidatePage.AddNewJobPostCandidate(newModel);


                //-----------------------------------------------------------------------------------------
                //checking if vacancy of job completed or not, if vacancy fulfilled then deactivate the job

                var vacancy = _dbContext.JobPost.FirstOrDefault(x => x.ID == newModel.job_Id).vacancy;
                var count_post = _dbContext.JobPostCandidate.Where(x => x.job_Id == newModel.job_Id).Count();

                if (vacancy == count_post)
                {
                    var jobPostModel = await _jobPostPageservices.getJobPostById(newModel.job_Id);

                    //serializing the object into string
                    TempData["candidate"] = JsonConvert.SerializeObject(latestRecord);
                    TempData["jobpost"] = JsonConvert.SerializeObject(jobPostModel);

                    return RedirectToAction("UpdateJobPostPost", "JobPost", new { model = jobPostModel, vacancy_overflow = true });
                }
                //--------------------------------------------------------------------------------------------
                return RedirectToAction("SendOTPToMail", model);
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Something went wrong !! Please try again later.";
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// This method is for sending mail just put an smtp according to your mail server  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendOTPToMail(CandidateViewModel model, bool vacancy_overflow = false)
        {
            //generate otp
            string body = GenerateToken();

            if (vacancy_overflow)
            {
                //getting candidate model for sending to confirm otp
                CandidateViewModel candidateModel = new CandidateViewModel();
                candidateModel = JsonConvert.DeserializeObject<CandidateViewModel>((string)TempData["candidateForEmailConfirmation"]);


                UserEmailOptions options_new = new UserEmailOptions
                {
                    Subject = "Recruitment Portal : Confirm you Email for verifying your Application.",
                    ToEmails = new List<string>() { candidateModel.email },
                    Body = body
                };
                //sending mail to Receivers
                try
                {
                    await _emailService.SendTestEmail(options_new);
                    ViewData["token"] = body;
                    ViewData["email"] = candidateModel.email;
                }
                catch (Exception ex)
                {
                    ViewData["error"] = "error";
                }
                return View(candidateModel);
            }
            else
            {
                UserEmailOptions optionss = new UserEmailOptions
                {
                    Subject = "Recruitment Portal : Confirm you Email for verifying your Application.",
                    ToEmails = new List<string>() { model.email },
                    Body = body
                };
                //sending mail to Receivers
                try
                {
                    await _emailService.SendTestEmail(optionss);
                    ViewData["token"] = body;
                    ViewData["email"] = model.email;
                }
                catch (Exception ex)
                {
                    ViewData["error"] = "error";
                }
            }
            OTPViewModel otpModel = new OTPViewModel();
            otpModel.email = model.email;
            otpModel.token = Convert.ToDouble(body);

            return RedirectToAction("otpView",otpModel);
        }
        [HttpGet]
        public IActionResult OtpView(OTPViewModel model)
        {
            return View(model);
        }
        //if OTP entered Successfully
        /// <summary>
        /// if OTP entered Successfully this action will return true
        /// </summary>
        /// <param name="email_ID"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtpViewPost(OTPViewModel model)
        {
            if (model.otp != model.token)
            {
                ModelState.AddModelError("otp", "Invalid OTP");
                return View("OtpView",model);
            }
            var FinalData = await _candidatePageServices.getCandidateByEmailId(model.email);
            FinalData.emailConfirmed = true;
            await _candidatePageServices.UpdateCandidate(FinalData);

            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = Messages.SuccessfullyApplied;
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// FUNCTION WILL GENERATE A RANDOM VALUE AND PASS AS OTP
        /// </summary>
        /// <returns></returns>
        public string GenerateToken()
        {
            //sending otp on email.========================================================================================
            Random rnd = new Random();
            string otp = rnd.Next(1000, 9999).ToString();
            return otp;
        }

        
        #endregion


        #region private methods
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

        //public List<string> noticePeriodList()
        //{
        //    List<string> periodList = new List<string>();
        //    periodList.Add("Select");
        //    periodList.Add("Immediate");
        //    periodList.Add("15 Days");
        //    periodList.Add("30 Days");
        //    periodList.Add("60 Days");
        //    periodList.Add("90 Days");
        //    return periodList;
        //}
        #endregion


    }
}
