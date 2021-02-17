using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class CandidateController : Controller
    {
        IQueryable<CandidateViewModel> candidateList;
        CandidateViewModel FinalData = new CandidateViewModel();
        private readonly ICandidatePage _candidatePageServices;
        private readonly IDegreePage _degreePageServices;
        private readonly IDepartmentPage _departmentPageservices;
        private readonly IJobPostCandidatePage _jobPostCandidatePage;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;


        //for dropdown
        private readonly RecruitmentPortalDbContext _dbContext;

        public IEmailService _emailService { get; }

        public CandidateController(IWebHostEnvironment environment,
             ICandidatePage candidatePage,
             IDegreePage degreePageServices,
             RecruitmentPortalDbContext dbContext,
              IJobPostCandidatePage jobPostCandidatePage,
             IDepartmentPage departmentPageservices,
             IEmailService emailService,
             UserManager<ApplicationUser> userManager)
        {
            _candidatePageServices = candidatePage;
            _environment = environment;
            _userManager = userManager;
            _dbContext = dbContext;
            _jobPostCandidatePage = jobPostCandidatePage;
            _degreePageServices = degreePageServices;
            _departmentPageservices = departmentPageservices;
            _emailService = emailService;
        }

        /// <summary>
        /// This method Retrieves all Applications from the database
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public async Task<IActionResult> Applications(string SearchString)
        {
            IQueryable<CandidateViewModel> modelList;
            modelList = await _candidatePageServices.getCandidates();

            try
            {
                List<CandidateViewModel> newList = new List<CandidateViewModel>();
                newList = modelList.ToList();
                foreach (var item in newList)
                {
                    JobPostCandidate model = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == item.ID).FirstOrDefault();
                    item.jobName = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == model.job_Id).job_title;
                }
                modelList = newList.AsQueryable();


                //for only verified candidate applications
                modelList = modelList.Where(x => x.emailConfirmed == true);

                //Added search box test
                if (!String.IsNullOrEmpty(SearchString))
                {
                    modelList = modelList.Where(s => s.jobName.ToUpper().Contains(SearchString.ToUpper()) || s.degree.ToUpper().Contains(SearchString.ToUpper()) || s.experience.ToUpper().Contains(SearchString.ToUpper()));
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return View(modelList);
        }


        /// <summary>
        /// This method retrieves Application-Form which is used to apply for a particular job [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id) //jobpost id
        {


            //creating a candidate applying for job
            CandidateViewModel model = new CandidateViewModel();
            model.jobpostID = id;

            //------------------------------------------------------------------------
            //fetching all the degrees for candidate to apply with.

            //getting data from database
            List<Degree> DegreeList = new List<Degree>();
            DegreeList = (from element in _dbContext.Degree select element).ToList();

            //inserting into dropdown list
            DegreeList.Insert(0, new Degree { ID = 0, degree_name = "Select Degree" });

            //assigning degreelist to viewbag.listofdegree
            ViewBag.ListOfDegree = DegreeList;
            //--------------------------------------------------------------------------

            return View(model);
        }
        /// <summary>
        /// This method stores the applicant's form-data to the database [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(CandidateViewModel model)
        {
            //checking if already applied for same job
            candidateList = await _candidatePageServices.getCandidates();

            try
            {
                List<CandidateViewModel> list = new List<CandidateViewModel>();
                list = candidateList.ToList();
                foreach (var item in list)
                {
                    var job_ID = _dbContext.JobPostCandidate.AsNoTracking().FirstOrDefault(x => x.candidate_Id == item.ID).job_Id;
                    if (item.email == model.email && model.jobpostID == job_ID)
                    {
                        TempData["msg1"] = model.email;
                        return RedirectToAction("Index", "Home", new { s = TempData["msg1"] });
                    }
                }


                //------------------------------------------------------------------------------------
                //receiving dropdown value of degree
                if (model.degree == "Select Degree")
                {
                    ModelState.AddModelError("", "Select Degree");
                }
                //getting data from database
                List<Degree> DegreeList = new List<Degree>();
                DegreeList = (from element in _dbContext.Degree select element).ToList();

                //inserting into dropdown list
                DegreeList.Insert(0, new Degree { ID = 0, degree_name = "Select Degree" });

                //assigning degreelist to viewbag.listofdegree
                ViewBag.ListOfDegree = DegreeList;
                //------------------------------------------------------------------------------------


                //getting selected value for degree
                DegreeViewModel selectedDegree = await _degreePageServices.getDegreeById(model.selectedDegree);
                string degreename = selectedDegree.degree_name;


                DepartmentViewModel selectedDept = await _departmentPageservices.getDepartmentById(model.selectDept);
                string deptename = selectedDept.dept_name;

                model.degree = degreename + "(" + deptename + ")";


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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("SendOTPToMail", model);
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
            //Inserting Select item in List
            DepartmentList.Insert(0, new Department { ID = 0, dept_name = "Select Department" });

            return Json(new SelectList(DepartmentList, "ID", "dept_name"));
        }

        /// <summary>
        /// This method is for sending mail just put an smtp according to your mail server  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendOTPToMail(CandidateViewModel model)
        {
            
            //generate otp
            string body = GenerateToken();

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
        [HttpGet]
        public async Task<bool> EmailConfirmation(string email_ID)
        {
            FinalData = await _candidatePageServices.getCandidateByEmailId(email_ID);
            FinalData.emailConfirmed = true;
            await _candidatePageServices.UpdateCandidate(FinalData);
            ViewBag.msg = "Email Confirmed !";
            return true;
        }

        /// <summary>
        /// This method fetches details of particular candidate when Details button gets clicked
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            CandidateViewModel item = await _candidatePageServices.getCandidateById(id);
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
            //return File(path, MediaTypeNames.Application.Octet, Path.GetFileName(path));

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


    }
}
