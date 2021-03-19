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
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
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
              IJobPostPage jobPostPageservices,
             IDegreePage degreePageServices,
             RecruitmentPortalDbContext dbContext,
              IJobPostCandidatePage jobPostCandidatePage,
             IDepartmentPage departmentPageservices,
             IEmailService emailService,
             UserManager<ApplicationUser> userManager)
        {
            _candidatePageServices = candidatePage;
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
        //public async Task<IActionResult> AllApplications(string SearchString, string Application_mode)
        //{
        //    if (Application_mode == mode_Pending)
        //    {
        //        return RedirectToAction("PendingApplications", "Candidate", new { Application_mode = Application_mode });
        //    }
        //    if (Application_mode == mode_Active)
        //    {
        //        return RedirectToAction("Index", "JobApplication", new { Application_mode = Application_mode });
        //    }
        //    if (Application_mode == mode_Selected)
        //    {
        //        return RedirectToAction("SelectedJobApplications", "JobApplication", new { Application_mode = Application_mode });
        //    }
        //    if (Application_mode == mode_Rejected)
        //    {
        //        return RedirectToAction("RejectedJobApplications", "JobApplication", new { Application_mode = Application_mode });
        //    }

        //    else
        //    {
        //        IQueryable<CandidateViewModel> modelList;
        //        modelList = await _candidatePageServices.getCandidates();

        //        //creating a select list for selecting status type applications
        //        List<SelectListItem> ObjItem = new List<SelectListItem>()
        //        {
        //          new SelectListItem {Text="All Applications",Value = mode_All},
        //          new SelectListItem {Text="New",Value = mode_Pending},
        //          new SelectListItem {Text="Active",Value = mode_Active},
        //          new SelectListItem {Text="Selected",Value = mode_Selected},
        //          new SelectListItem {Text="Rejected",Value = mode_Rejected},
        //        };
        //        ViewBag.menuSelect = ObjItem;

        //        try
        //        {
        //            List<CandidateViewModel> newList = new List<CandidateViewModel>();
        //            newList = modelList.ToList();
        //            foreach (var item in newList)
        //            {
        //                JobPostCandidate model = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault();
        //                item.jobName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
        //                item.jobRole = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_role;
        //                item.isActive = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).Any();
        //                item.isSelected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Complete).Any();
        //                item.isRejected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Rejected).Any();

        //                //getting job app id of candidates who have their records in job application table
        //                var proceededCandidates = _dbContext.jobApplications.ToList();
        //                var isPresent = proceededCandidates.Where(x => x.candidateId == item.ID).Any();
        //                if (isPresent)
        //                {
        //                    item.JobAppId = proceededCandidates.Where(x => x.candidateId == item.ID).FirstOrDefault().ID;
        //                }
        //            }
        //            modelList = newList.AsQueryable();


        //            //for only verified candidate applications
        //            modelList = modelList.Where(x => x.emailConfirmed == true);

        //            //Added search box test
        //            if (!String.IsNullOrEmpty(SearchString))
        //            {
        //                modelList = modelList.Where(s => s.jobName.ToUpper().Contains(SearchString.ToUpper()) || s.degree.ToUpper().Contains(SearchString.ToUpper()) || s.experience.ToUpper().Contains(SearchString.ToUpper()) || s.name.ToUpper().Contains(SearchString.ToUpper()));
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //        return View(modelList);
        //    }
        //}

        public ActionResult AllApplications()
        {
            ViewBag.DegreeList = SelectionList.GetDegreeList().Select(m => new { Id = m.ID, Name = m.degree_name });
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
                    item.jobName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
                    item.jobRole = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_role;
                    item.isActive = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).Any();
                }
                modelList = newList.AsQueryable();


                //for only verified candidate applications
                modelList = modelList.Where(x => x.emailConfirmed == true && x.isActive == false);

                //Added search box test
                if (!String.IsNullOrEmpty(SearchString))
                {
                    modelList = modelList.Where(s => s.jobName.ToUpper().Contains(SearchString.ToUpper()) || s.degree.ToUpper().Contains(SearchString.ToUpper()) || s.experience.ToUpper().Contains(SearchString.ToUpper()) || s.name.ToUpper().Contains(SearchString.ToUpper()));
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            ////for sorting
            //int pageSize = 4;
            //return View(await PaginatedList<CandidateViewModel>.CreateAsync(modelList.AsNoTracking(), pageNumber ?? 1, pageSize));
            return View(modelList);
        }

        /// <summary>
        /// This method retrieves Application-Form which is used to apply for a particular job [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) //jobpost id
        {

            CandidateViewModel model = new CandidateViewModel();
            try
            {
                model.jobpostID = Convert.ToInt32(RSACSPSample.DecodeServerName(id));

                model.jobName = _dbContext.JobPost.FirstOrDefault(x => x.ID == model.jobpostID).job_title;
                //------------------------------------------------------------------------
                //fetching all the degrees for candidate to apply with.

                //getting data from database
                List<Degree> DegreeList = new List<Degree>();
                DegreeList = (from element in _dbContext.Degree select element).ToList();
                DegreeList = DegreeList.Where(x => x.isActive == true).ToList();

                //inserting into dropdown list
                DegreeList.Insert(0, new Degree { ID = 0, degree_name = "Select Degree" });

                //assigning degreelist to viewbag.listofdegree
                ViewBag.ListOfDegree = DegreeList;

                //creating a select list for selecting Hear about US field
                List<SelectListItem> apply_through_items = new List<SelectListItem>()
                    {
                      new SelectListItem {Text="Select",Value="null"},
                      new SelectListItem {Text="LinkedIn",Value="LinkedIn"},
                      new SelectListItem {Text="Indeed",Value="Indeed"},
                      new SelectListItem {Text="Naukri.com",Value="Naukri.com"},
                      new SelectListItem {Text="Monster.com",Value="Monster.com"},
                      new SelectListItem {Text="Reference",Value="Reference"},
                      new SelectListItem {Text="Other",Value="Other"}
                    };
                ViewBag.ReferenceSelect = apply_through_items;
                //--------------------------------------------------------------------------
            }
            catch (Exception ex)
            {

                model = new CandidateViewModel();
            }
            return View(model);
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
                    //var job_ID = _dbContext.JobPostCandidate.FirstOrDefault(x => x.candidate_Id == item.ID).job_Id;
                    //var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault().job_Id;
                    if (item.email == model.email)
                    {
                        TempData["msg1"] = model.email;
                        return RedirectToAction("Index", "Home", new { s = TempData["msg1"] });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                model.apply_date = DateTime.Now;

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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("SendOTPToMail", model);
        }

        //    await _jobPostPageservices.UpdateJobPost(jobPostModel);
        //    return RedirectToAction("SendOTPToMail", model);
        //}
        




            ////For Rejecting New Candidate Application without Proceeding it
            public async Task<IActionResult> RejectApplicant(string Cid)
        {
            int decrypted_key = Convert.ToInt32(RSACSPSample.DecodeServerName(Cid));

            CandidateViewModel candidateModel = new CandidateViewModel();
            candidateModel = await _candidatePageServices.getCandidateById(decrypted_key);

            candidateModel.isRejected = true;

            //AddNewCandidate
            // await _candidatePageServices.UpdateCandidate(candidateModel);



            //int decrypted_key = Convert.ToInt32(RSACSPSample.DecodeServerName(Cid));

            //return RedirectToAction("SendRejectionMailToCandidate", "Candidate", new { id = candidateModel.ID });

            return RedirectToAction("AllApplications", "Candidate");
        }

        /// <summary>
        /// This method returns the Department according to Selected Degree
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetDept(int Id)
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
                    ToEmails = new List<string>() { candidateModel.email},
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
            return true;
        }

        /// <summary>
        /// For sending email after rejecting a candidate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendRejectionMailToCandidate(int id)
        {

            CandidateViewModel model = await _candidatePageServices.getCandidateById(id);

            //Creating Email Credentials - email-id, subject
            UserEmailOptions emailOptions = new UserEmailOptions
            {
                Subject = "Your application to Shaligram Infotech",
                ToEmails = new List<string>() { model.email },
                Body = "Dear  " + model.name + "," + "<br />" + "Sorry ! But We are not having any position you're Applying for." + "<br/>" + " We will get back to you if there is any vacancy for you."
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
            return RedirectToAction("AllApplications", "Candidate", new { Application_mode = status_Rejected });
        }

        /// <summary>
        /// This method fetches details of particular candidate when Details button gets clicked
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id, bool backToAll)
        {
            if (backToAll)
            {
                ViewBag.backToAll = backToAll;
            }
            int actualId = Convert.ToInt32(RSACSPSample.Decrypt(id));
            CandidateViewModel item = await _candidatePageServices.getCandidateById(actualId); 
            return View(item);
        }


        /// <summary>
        /// This method is used for downloading Candidate's Resume/CV into system.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(string filename)
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


        public async Task<IActionResult> GetApplicationsList(string startDate, string endDate, string applicationType, string degree)
        {
            if (applicationType == null) applicationType = string.Empty;
            if (degree == null) degree = string.Empty;
            DateTime? sDate = !string.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : (DateTime?)null;
            DateTime? eDate = !string.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : (DateTime?)null;

            IQueryable<CandidateViewModel> modelList;
            List<CandidateViewModel> newList = new List<CandidateViewModel>();
            List<CandidateViewModel> filteredList = new List<CandidateViewModel>();

            try
            {
                modelList = await _candidatePageServices.getCandidates();
                newList = modelList.ToList();
                foreach (var item in newList)
                {
                    item.EncryptedId = HttpUtility.UrlEncode(RSACSPSample.Encrypt(item.ID));
                    JobPostCandidate model = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault();
                    item.jobName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
                    item.jobRole = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_role;
                    item.isActive = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).Any();
                    item.isSelected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Complete).Any();
                    item.isRejected = _dbContext.jobApplications.Where(x => x.candidateId == item.ID && x.status == status_Rejected).Any();

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
        #endregion

        #region Private Methods
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
        #endregion
    }
}
