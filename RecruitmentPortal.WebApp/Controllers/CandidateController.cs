using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class CandidateController : BaseController
    {
        #region private variables
        IQueryable<CandidateViewModel> candidateList;

        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private string status_Complete = Enum.GetName(typeof(JobApplicationStatus), 2);
        private string status_Rejected = Enum.GetName(typeof(JobApplicationStatus), 3);

        //Application mode
        private string mode_All = Enum.GetName(typeof(ApplicationMode), 1);
        private string mode_Pending = Enum.GetName(typeof(ApplicationMode), 2);
        private string mode_Active = Enum.GetName(typeof(ApplicationMode), 3);
        private string mode_Selected = Enum.GetName(typeof(ApplicationMode), 4);
        private string mode_Rejected = Enum.GetName(typeof(ApplicationMode), 5);

        CandidateViewModel FinalData = new CandidateViewModel();
        private readonly ICandidatePage _candidatePageServices;
        private readonly IJobApplicationPage _jobApplicationPage;
        private readonly IDegreePage _degreePageServices;
        private readonly IDepartmentPage _departmentPageservices;
        private readonly IJobPostCandidatePage _jobPostCandidatePage;
        private readonly IJobPostPage _jobPostPageservices;

        //for userid
        private readonly UserManager<ApplicationUser> _userManager;
        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        //for dropdown
        private readonly RecruitmentPortalDbContext _dbContext;
        public IEmailService _emailService { get; }
        #endregion

        #region Constructor
        public CandidateController(IWebHostEnvironment environment,
             ICandidatePage candidatePage,
             IJobApplicationPage jobApplicationPage,
              IJobPostPage jobPostPageservices,
             IDegreePage degreePageServices,
             RecruitmentPortalDbContext dbContext,
              IJobPostCandidatePage jobPostCandidatePage,
             IDepartmentPage departmentPageservices,
             IEmailService emailService,
             UserManager<ApplicationUser> userManager)
        {
            _candidatePageServices = candidatePage;
            _jobApplicationPage = jobApplicationPage;
            _environment = environment;
            _jobPostPageservices = jobPostPageservices;
            _userManager = userManager;
            _dbContext = dbContext;
            _jobPostCandidatePage = jobPostCandidatePage;
            _degreePageServices = degreePageServices;
            _departmentPageservices = departmentPageservices;
            _emailService = emailService;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method Retrieves all Applications from the database
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public ActionResult AllApplications(string ApplicationType)
        {
            ViewBag.DegreeList = SelectionList.GetDegreeList().Select(m => new { Id = m.ID, Name = m.degree_name });

            if (ApplicationType != null)
                ViewBag.ApplicationType = ApplicationType;

            return View();
        }
        public async Task<IActionResult> PendingApplications(string SearchString, string Application_mode)
        {
            IQueryable<CandidateViewModel> modelList;
            modelList = await _candidatePageServices.getCandidates();

            //creating a select list for selecting status type applications
            List<SelectListItem> ObjItem = new List<SelectListItem>()
                    {
                      new SelectListItem {Text="All Applications",Value = mode_All},
                      new SelectListItem {Text="New",Value = mode_Pending},
                      new SelectListItem {Text="Active",Value = mode_Active},
                      new SelectListItem {Text="Selected",Value = mode_Selected},
                      new SelectListItem {Text="Rejected",Value = mode_Rejected},
                    };
            ViewBag.menuSelect = ObjItem;

            try
            {
                List<CandidateViewModel> newList = new List<CandidateViewModel>();
                newList = modelList.ToList();
                foreach (var item in newList)
                {
                    JobPostCandidate model = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault();
                    item.jobpostName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
                    item.jobRole = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_role;
                    item.isActive = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).Any();
                }
                modelList = newList.AsQueryable();


                //for only verified candidate applications
                modelList = modelList.Where(x => x.emailConfirmed == true && x.isActive == false);

                //Added search box test
                if (!String.IsNullOrEmpty(SearchString))
                {
                    modelList = modelList.Where(s => s.jobpostName.ToUpper().Contains(SearchString.ToUpper()) || s.degree.ToUpper().Contains(SearchString.ToUpper()) || s.experience.ToUpper().Contains(SearchString.ToUpper()) || s.name.ToUpper().Contains(SearchString.ToUpper()));
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return View(modelList);
        }
      

        /// <summary>
        /// This method stores the applicant's form-data to the database [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndexPost(CandidateViewModel model)
        {
            //checking if already applied for same job
            candidateList = await _candidatePageServices.getCandidates();

            try
            {
                List<CandidateViewModel> list = new List<CandidateViewModel>();
                list = candidateList.ToList();
                foreach (var item in list)
                {
                    if (item.email == model.email)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = model.email + " already exists !!";
                        return RedirectToAction("Index", "Home");
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
                model.apply_date = Convert.ToDateTime(DateTime.Now.ToString("G"));

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

                    return RedirectToAction("UpdateJobPostPost","JobPost", new { model = jobPostModel, vacancy_overflow = true });
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

            UserEmailOptions options = new UserEmailOptions
            {
                Subject = "Recruitment Portal : Confirm you Email for verifying your Application.",
                ToEmails = new List<string>() { model.email },
                Body = body
            };
            //sending mail to Receivers
            try
            {
                await _emailService.SendTestEmail(options);
                ViewData["token"] = body;
                ViewData["email"] = model.email;
            }
            catch (Exception ex)
            {
                ViewData["error"] = "error";
            }
            return View(model);
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

        //if OTP entered Successfully
        /// <summary>
        /// if OTP entered Successfully this action will return true
        /// </summary>
        /// <param name="email_ID"></param>
        /// <returns></returns>

        public async Task<bool> EmailConfirmation(string email_ID)
        {
            FinalData = await _candidatePageServices.getCandidateByEmailId(email_ID);
            FinalData.emailConfirmed = true;
            await _candidatePageServices.UpdateCandidate(FinalData);
            ViewBag.msg = "Email Confirmed !";
            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = Messages.SuccessfullyApplied;
            return true;
        }




        //For Rejecting New Candidate Application without Proceeding it
        public async Task<IActionResult> RejectNewApplicant(RejectReasonViewModel model)
        {
            CandidateViewModel candidateModel = new CandidateViewModel();
            if (model.CandidateId != null)
            {
                int decrypted_key = Convert.ToInt32(RSACSPSample.DecodeServerName(model.CandidateId));
                candidateModel = await _candidatePageServices.getCandidateById(decrypted_key);
                candidateModel.is_InitReject = true;
            }

            return RedirectToAction("UpdateCandidate", candidateModel);
        }
        public async Task<IActionResult> UpdateCandidate(CandidateViewModel model)
        {
            await _candidatePageServices.UpdateCandidate(model);
            return RedirectToAction("SendRejectionMailToCandidate", "Candidate", new { id = model.ID });
        }


        /// <summary>
        /// For Rejecting Active Job Application 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> RejectApplicant(RejectReasonViewModel model)
        {
            CandidateViewModel candidateViewModel = new CandidateViewModel();
            if (model.CandidateId != null)
            {
                int decrypted_key = Convert.ToInt32(RSACSPSample.DecodeServerName(model.CandidateId));
                candidateViewModel = await _candidatePageServices.getCandidateById(decrypted_key);
                candidateViewModel.isRejected = true;
                
                int job_ID;
                job_ID = _dbContext.jobApplications.AsNoTracking().FirstOrDefault(x => x.candidateId == decrypted_key).ID;
                model.JobAppId = job_ID;
            }
            return RedirectToAction("UpdateJobApplication", "JobApplication", new { id = RSACSPSample.EncodeServerName(model.JobAppId.ToString()), rejected = true});
           // return RedirectToAction("AllApplications", "Candidate");
        }

        /// <summary>
        /// For sending email after rejecting a candidate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendRejectionMailToCandidate(int id)
        {

            CandidateViewModel model = await _candidatePageServices.getCandidateById(id);

            //preparing email body 
            string Path = _environment.WebRootPath + "/Templates/RejectingCandidateTemplate.html";
            string bodyTemplate = System.IO.File.ReadAllText(Path);

            bodyTemplate = bodyTemplate.Replace("[@CandidateName]", model.name);


            //Creating Email Credentials - email-id, subject
            UserEmailOptions emailOptions = new UserEmailOptions
            {
                Subject = "Your application to Shaligram Infotech",
                ToEmails = new List<string>() { model.email },
                Body = bodyTemplate
            };

            //Sending Email to Receiver
            try
            {
                await _emailService.SendTestEmail(emailOptions);
                ViewData["email"] = model.email;
            }
            catch (Exception ex)
            {
                ViewData["error"] = "error";
            }
            return RedirectToAction("AllApplications", "Candidate");
        }

        /// <summary>
        /// This method fetches details of particular candidate when Details button gets clicked
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            CandidateViewModel item = new CandidateViewModel();

            int actualId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            item = await _candidatePageServices.getCandidateById(actualId);
            item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
            return View(item);
        }


        /// <summary>
        /// This method is used for downloading Candidate's Resume/CV into system.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                if (filename == null)
                    return Content("filename not present");

                var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot" + @"\Resume", filename);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (FileNotFoundException ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Cound not find requested file.";
                throw new Exception("Cound not find requested file.");
                //RedirectToAction("Details", "Candidate", new { id = id });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "There is a problem with downloading file";
                throw new Exception("There is a problem with downloading file");
                //RedirectToAction("Details", "Candidate", new { id = id });
            }
        }




        public async Task<IActionResult> GetApplicationsList(string startDate, string endDate, string applicationType, string degree)
        {
            IQueryable<CandidateViewModel> modelList;
            List<CandidateViewModel> newList = new List<CandidateViewModel>();
            List<CandidateViewModel> filteredList = new List<CandidateViewModel>();
            try
            {
                if (applicationType == null) applicationType = string.Empty;
                if (degree == null) degree = string.Empty;
                DateTime? sDate = !string.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : (DateTime?)null;
                DateTime? eDate = !string.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : (DateTime?)null;

                modelList = await _candidatePageServices.getCandidates();
                newList = modelList.Where(x => x.is_InitReject == false && x.emailConfirmed == true).ToList();
                foreach (var item in newList)
                {
                    item.EncryptedId = HttpUtility.UrlEncode(RSACSPSample.EncodeServerName(item.ID.ToString()));
                    JobPostCandidate model = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault();
                    item.jobpostName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
                    item.jobRole = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_role;
                    item.isActive = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).Any();
                    item.isSelected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Complete).Any();
                    item.isRejected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Rejected).Any();
                    item.FormattedDate = item.apply_date.ToString("dd-MM-yyyy");
                    //getting job app id of candidates who have their records in job application table
                    var proceededCandidates = _dbContext.jobApplications.ToList();
                    var isPresent = proceededCandidates.Where(x => x.candidateId == item.ID).Any();
                    if (isPresent)
                    {
                        item.JobAppId = proceededCandidates.Where(x => x.candidateId == item.ID).FirstOrDefault().ID;
                    }
                    if (item.isActive == true && item.isSelected == true)
                    {
                        item.JobStatus = "Selected";
                    }
                    else if(item.isActive == true && item.isSelected == false && item.isRejected == false)
                    {
                        item.JobStatus = "Active";
                    }
                    else if(item.isRejected == true)
                    {
                        item.JobStatus = "Rejected";
                    }
                    else if(item.isActive == false && item.isSelected == false)
                    {
                        item.JobStatus = "New";
                    }
                }
                filteredList = newList.Where(m => (m.JobStatus.Contains(applicationType) || applicationType == null)
                                        && (m.degree.Contains(degree) || degree == null)
                                        && (m.apply_date >= sDate || sDate == null)
                                        && (m.apply_date <= eDate || eDate == null)).ToList();

                return Json(new { data = filteredList });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = filteredList });
            }
        }

        /// <summary>
        /// FOR SHOWING LIST OF ACTIVE APPLICATIONS
        /// </summary>
        /// <param name="Application_mode"></param>
        /// <param name="istatus"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetActiveApplicationsList(string startDate, string endDate, string degree, string applicationType, string istatus = "Pending")
        {
            if (applicationType == null) applicationType = string.Empty;
            if (degree == null) degree = string.Empty;
            if (applicationType == "Active") applicationType = "Pending";
            DateTime? sDate = !string.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : (DateTime?)null;
            DateTime? eDate = !string.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : (DateTime?)null;

            IQueryable<JobApplicationViewModel> modelList;
            List<Schedules> mylist = new List<Schedules>();
            List<JobApplicationViewModel> newlist = new List<JobApplicationViewModel>();
            List<JobApplicationViewModel> filteredList = new List<JobApplicationViewModel>();

            try
            {
                modelList = await _jobApplicationPage.getJobApplications();
                //    modelList = modelList.Where(x => x.status == status_Pending);
                newlist = modelList.OrderByDescending(x => x.start_date).ToList();

                foreach (var data in newlist)
                {
                    data.EncryptedJobId = HttpUtility.UrlEncode(RSACSPSample.EncodeServerName(data.ID.ToString()));
                    data.EncryptedId = HttpUtility.UrlEncode(RSACSPSample.EncodeServerName(data.candidateId.ToString()));
                    data.candidateName = getCandidateNameById(data.candidateId);
                    data.position = getPositionByCandidateId(data.candidateId);
                    data.job_Role = getJobRoleByCandidateId(data.candidateId);
                    data.interview_Status = getInterviewStatusByCandidateId(data.candidateId);
                    data.FormattedDate = data.start_date.ToString("dd-MM-yyyy");          
                }
             
                //newlist = newlist.Where(x => x.interview_Status == istatus).ToList();
                filteredList = newlist.Where(m => (m.status.Contains(applicationType) || applicationType == null)
                                               && (degree == string.Empty)
                                               && (m.start_date >= sDate || sDate == null)
                                               && (m.start_date <= eDate || eDate == null)).ToList();

                //var finallist = filteredList.Select(m => new { m.EncryptedJobApplicationId, m.EncryptedId, m.roundName, m.rating, m.statusName, m.remark, m.datetime, m.InterviewerNames }).ToList();


                return Json(new { data = filteredList });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = filteredList });
            }
        }
        //public DateTime changeFormat(DateTime value)
        //{
        //    CultureInfo provider = CultureInfo.InvariantCulture;
        //    var x = value.ToString();
        //    var result = DateTime.ParseExact(x, "dd-MM-yyyy",provider);
        //    return result;
        //}

        /// <summary>
        /// FOR SHOWING LIST OF SELECTED JOB-APPLICATIONS
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SelectedJobApplications(string startDate, string endDate, string degree, string applicationType)
        {
            if (applicationType == null) applicationType = string.Empty;
            if (degree == null) degree = string.Empty;
            if (applicationType == "Selected") applicationType = "Accepted";
            DateTime? sDate = !string.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : (DateTime?)null;
            DateTime? eDate = !string.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : (DateTime?)null;

            IQueryable<JobApplicationViewModel> models = null;
            List<JobApplicationViewModel> newlist = new List<JobApplicationViewModel>();
            List<JobApplicationViewModel> filteredlist = new List<JobApplicationViewModel>();
            CandidateViewModel candidateModel = new CandidateViewModel();

            try
            {
                models = await _jobApplicationPage.getJobApplications();
                models = models.Where(x => x.status == status_Complete);
                //getting candidate name and job position with candidateId of JobApplication model
                newlist = models.OrderByDescending(m => m.joining_date).ToList();
                foreach (var item in newlist)
                {
                    item.EncryptedId = RSACSPSample.EncodeServerName(item.ID.ToString());
                    item.candidateName = getCandidateNameById(item.candidateId);
                    item.position = getPositionByCandidateId(item.candidateId);
                    item.job_Role = getJobRoleByCandidateId(item.candidateId);
                    item.FormattedDate = item.joining_date.ToString("dd-MM-yyyy");
                }
                filteredlist = newlist.Where(m => (m.status.Contains(applicationType) || applicationType == null)
                                              && (degree == string.Empty)
                                              && (m.joining_date >= sDate || sDate == null)
                                              && (m.joining_date <= eDate || eDate == null)).ToList();

                return Json(new { data = filteredlist });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = filteredlist });
            }
        }

        /// <summary>
        /// List of Rejected Candidates.
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="Application_mode"></param>
        /// <returns></returns>
        public async Task<IActionResult> RejectedJobApplications(string startDate, string endDate, string degree, string applicationType)
        {
            if (applicationType == null) applicationType = string.Empty;
            if (degree == null) degree = string.Empty;
            DateTime? sDate = !string.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : (DateTime?)null;
            DateTime? eDate = !string.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : (DateTime?)null;

            IQueryable<JobApplicationViewModel> models = null;
            List<JobApplicationViewModel> newList = new List<JobApplicationViewModel>();
            List<JobApplicationViewModel> filteredlist = new List<JobApplicationViewModel>();

            try
            {
                models = await _jobApplicationPage.getJobApplications();
                models = models.Where(x => x.status == status_Rejected);
                newList = models.ToList();

                foreach (var item in newList)
                {
                    item.EncryptedId = HttpUtility.UrlEncode(RSACSPSample.EncodeServerName(item.ID.ToString()));
                    item.candidateName = getCandidateNameById(item.candidateId);
                    item.position = getPositionByCandidateId(item.candidateId);
                    item.job_Role = getJobRoleByCandidateId(item.candidateId);
                    item.candidate = await _candidatePageServices.getCandidateById(item.candidateId);
                    item.FormattedDate = item.rejection_date.ToString("dd-MM-yyyy");
                }

                filteredlist = newList.Where(m => (m.status.Contains(applicationType) || applicationType == null)
                                              && (degree == string.Empty)
                                              && (m.rejection_date >= sDate || sDate == null)
                                              && (m.rejection_date <= eDate || eDate == null)).ToList();
                return Json(new { data = filteredlist });
            }

            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = filteredlist });
            }
        }

        #endregion

        #region Private Methods

        public async Task<IActionResult> getJobApplicationId(string id, bool flag)
        {
            var value =  _dbContext.jobApplications.Where(x => x.candidateId == Convert.ToInt32(RSACSPSample.DecodeServerName(id))).FirstOrDefault().ID;
            if (flag)
            {
                return RedirectToAction("Details", "JobApplication", new { id = RSACSPSample.EncodeServerName(value.ToString()) });

            }
            return RedirectToAction("SelectedJobApplicationsDetails", "JobApplication", new { id = RSACSPSample.EncodeServerName(value.ToString()) }) ;
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
        /// For Fetching Interview Status By Candidate Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getInterviewStatusByCandidateId(int id)
        {

            var mylist = _dbContext.Schedules.Where(x => x.candidateId == id).ToList();
            string interview_status = null;
            if (mylist.Count() > 1)
            {
                var isAnyPending = mylist.Where(x => x.status == 2).Any();
                var isAllCompleted = mylist.All(x => x.status == 3);
                var isAllScheduled = mylist.All(x => x.status == 1);


                if (isAllScheduled)
                {
                    interview_status = Enum.GetName(typeof(StatusType), 1);
                }
                if (isAnyPending)
                {
                    interview_status = Enum.GetName(typeof(StatusType), 2);
                }
                if (isAllCompleted)
                {
                    interview_status = Enum.GetName(typeof(StatusType), 3);
                }
                else
                {
                    interview_status = Enum.GetName(typeof(StatusType), 2);
                }

            }
            else
            {
                if (mylist.Count == 0)
                {
                    interview_status = Enum.GetName(typeof(StatusType), 2);
                }
                else
                {
                    interview_status = Enum.GetName(typeof(StatusType), mylist.FirstOrDefault().status);
                }
            }
            //}
            return interview_status;
        }
        /// <summary>
        /// This method is a part of Download method. It restricts the resume that it should only in form of .pdf/.doc/.docx
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        /// <summary>
        /// This method is a part of Download method. It restricts the resume that it should only in form of .pdf/.doc/.docx
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".png", "image/png"}
            };
        }


        /// <summary>
        /// for Loading the popup for adding or edititng the Rejection reason.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<IActionResult> RenderNewRejectionView(string id)
        {
            RejectReasonViewModel model = new RejectReasonViewModel();
            model.CandidateId = id;
            return PartialView("_NewRejectionView", model);
        }





        #endregion
    }
}
