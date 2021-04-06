using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Resources;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        #region private variables
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);
        private string status_Complete = Enum.GetName(typeof(JobApplicationStatus), 2);
        readonly int reqValue = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Completed"));
        IQueryable<SchedulesViewModel> allScheduleList;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IAuthorize _authorize;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public IEmailService _emailService { get; }
        private readonly RecruitmentPortalDbContext _dbContext;
        //for uploading Images/File : media stuff
        private readonly IWebHostEnvironment _environment;
        private readonly ISchedulesPage _schedulesPage;
        private readonly IJobApplicationPage _jobApplicationPage;


        #endregion

        #region Constructor
        public AccountController(IPasswordHasher<ApplicationUser> passwordHash,
            ISchedulesPage schedulesPage, IJobApplicationPage jobApplicationPage,
            RecruitmentPortalDbContext dbContext, IWebHostEnvironment environment,
           UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            passwordHasher = passwordHash;
            _emailService = emailService;
            _dbContext = dbContext;
            _schedulesPage = schedulesPage;
            _jobApplicationPage = jobApplicationPage;
            _environment = environment;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This Method fetches all Users(Interviewers) and display the list of them
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string SearchString)
        {
            IQueryable<ApplicationUser> plist = null;
            try
            {
                //Added search box test
                plist = _userManager.Users;
                if (!String.IsNullOrEmpty(SearchString))
                {
                    plist = plist.Where(s => s.Email.ToUpper().Contains(SearchString.ToUpper()) || s.position.ToUpper().Contains(SearchString.ToUpper()) || s.skype_id.ToUpper().Contains(SearchString.ToUpper()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(plist);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            try
            {
                var isInUse = _dbContext.SchedulesUsers.Where(x => x.UserId == user.Id).Any();
                if (isInUse)
                {
                    TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "User is Active. Can't be Deleted..!!";
                    return RedirectToAction("Index");
                }
                if (user != null)
                {
                    //check if the user is not having any rounds scheduled for interview
                    //[Tobedone]

                    IdentityResult result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User Deleted Successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "User not Deleted..!!";
                    }
                }
                else
                {
                    ModelState.AddModelError(user.Email, "User Not Found");
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return View("Index", _userManager.Users);
        }

        public IActionResult GetAllUsers()
        {
            IQueryable<ApplicationUser> ulist;
            List<ApplicationUser> newList = new List<ApplicationUser>();

            try
            {
                ulist = _userManager.Users;
                newList = ulist.ToList();
                //foreach (var item in newList)
                //{
                //    // item.Id = HttpUtility.UrlEncode(RSACSPSample.Encrypt(item.Id));
                //    item.Id = Helpers.RSACSPSample.EncodeServerName(item.Id);
                //

                return Json(new { data = newList });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = newList });
            }

        }







        ///<summary>
        ///This method fetches Home page for Admin
        ///</summary>
        ///<returns></returns>
        ///
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
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }

            return View(collection);
        }


        public async Task<IActionResult> getAdminIndexScheduleData()
        {
            AdminPanelViewModel collection = new AdminPanelViewModel();
            JobApplications allSchedules = new JobApplications();
            List<SchedulesViewModel> schedulelist = new List<SchedulesViewModel>();
            List<SchedulesViewModel> viewScheduleList = new List<SchedulesViewModel>();

            try
            {
                //getting all the schedules of all candidates
                allScheduleList = await _schedulesPage.GetAllSchedules();

                //filtering the schedules for getting only the incompleted schedules (for active applications only)
                allScheduleList = allScheduleList.Where(x => x.status != reqValue);
                viewScheduleList = allScheduleList.ToList();

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


                    foreach (var obj in item.InterviewerNames)
                    {
                        item.allInterviewersNames += obj.Name + "<br>";

                    }

                    //creating new schedule list 
                    if (allSchedules.status == status_Pending)
                    {
                        schedulelist.Add(item);
                    }
                }
                //assigning to view passing list
                allScheduleList = schedulelist.AsQueryable();
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = allScheduleList });
            }
            return Json(new { data = allScheduleList });
        }
        public async Task<IActionResult> getAdminIndexNotificationData()
        {
            AdminPanelViewModel collection = new AdminPanelViewModel();
            try
            {
                var notifyJobApplications = await _jobApplicationPage.getJobApplications();
                collection.selected_application = notifyJobApplications.Where(x => x.status == status_Complete && x.notified == false).ToList();
                foreach (var item in collection.selected_application)
                {
                    item.candidateName = _dbContext.Candidate.Where(x => x.ID == item.candidateId).FirstOrDefault().name;
                    item.position = getPositionByCandidateId(item.candidateId);
                    item.job_Role = getJobRoleByCandidateId(item.candidateId);
                    item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = collection.selected_application });
            }
            return Json(new { data = collection.selected_application });
        }



        ///<summary>
        ///This method fetches Home page for Interviewer
        ///</summary>
        ///<returns></returns>
        public async Task<IActionResult> InterviewerIndex()
        {
            //Counter Part
            InterviewerPanelViewModel collection = new InterviewerPanelViewModel();


            var pending_schedules = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));

            //filtering the schedules for getting only the incompleted schedules

            collection.pending_schedules = pending_schedules.Where(x => x.status != reqValue).ToList();
            collection.PendingScheduleCount = pending_schedules.Where(x => x.status != reqValue).ToList().Count();
            collection.CompletedScheduleCount = pending_schedules.Where(x => x.status == reqValue).Count();

            return View(collection);
        }

        public async Task<IActionResult> getInterviewIndexScheduleData()
        {
            List<SchedulesViewModel> collection = new List<SchedulesViewModel>();
            try
            {
                var pending_schedules = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));
                collection = pending_schedules.Where(x => x.status != reqValue).ToList();
                foreach (var item in collection)
                {
                    item.jobRole = getJobRoleByCandidateId(item.candidateId);
                    item.roundName = Enum.GetName(typeof(RoundType), item.round);
                    //getting interviewer names of schedule
                    List<UserModel> listOfUser = getInterviewerNames(item.ID);
                    item.InterviewerNames = listOfUser;
                    item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = collection });
            }
            return Json(new { data = collection });

        }

        /// <summary>
        /// This method works for Registering the User [GET- Displays empty form]
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Register(string id)
        {
            ApplicationUserViewModel model = new ApplicationUserViewModel();
            if (!string.IsNullOrEmpty(id))
            {

                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.UserId = Guid.Parse(user.Id);
                    model.Email = user.Email;
                    model.UserName = user.UserName;
                    model.position = user.position;
                    model.skype_id = user.skype_id;
                    model.file = user.file;
                    //if (user.file != null)
                    //{
                    //    model.file = user.file;
                    //}
                    //if (user.file == null)
                    //    return Content("filename not present");

                    //var newname = user.file.Split("-")[0];
                    //var extension = user.file.Split(".")[1];
                    //newname += "."+extension;
                    //var path = Path.Combine(
                    //Directory.GetCurrentDirectory(), "wwwroot" + @"\Files", newname);

                    //using (var stream = System.IO.File.OpenRead(path))
                    //{
                    //    model.FileNew = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                    //}
                    //if (user.file != null)
                    //{
                    //    var path = Path.Combine(
                    //Directory.GetCurrentDirectory(), "wwwroot" + @"\Resume", user.file);

                    //    var memory = new MemoryStream();
                    //    using (var stream = new FileStream(path, FileMode.Open))
                    //    {
                    //        await stream.CopyToAsync(memory);
                    //    }
                    //    memory.Position = 0;
                    //    model.FileNew = File(memory, GetContentType(path), );
                    //    new FormFile(memory, 0, 0, "name", Path.GetFileName(path));
                    //}

                }
                model.editMode = true;
            }
            return View(model);
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
        /// This method works for Registering the User [POST- Send Data to Database(After the form getting filled)]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUserViewModel model, string submit)
        {
            if (model.editMode)
            {
                try
                {
                    string extension = null;
                    string basename = null;
                    string uploads = null;
                    if (model.file != null)
                    {
                        //File details fetching
                        //create a place in wwwroot for storing uploaded files
                        uploads = Path.Combine(_environment.WebRootPath, "Files");
                        
                        if (model.FileNew != null)
                        {
                            extension = Path.GetExtension(model.FileNew.FileName);
                            basename = Path.Combine(Path.GetDirectoryName(model.FileNew.FileName),
                                    Path.GetFileNameWithoutExtension(model.FileNew.FileName));
                        }
                        else
                        {
                            extension = Path.GetExtension(model.file);
                            basename = Path.Combine(Path.GetDirectoryName(model.file),
                                    Path.GetFileNameWithoutExtension(model.file));
                        }
                    
                        //if (model.FileNew != null)
                        //{
                        //    Guid gid = Guid.NewGuid();
                        //    string uniquefilename = basename + "-" + gid + extension;
                        //    using (var fileStream = new FileStream(Path.Combine(uploads, uniquefilename), FileMode.Create))
                        //    {
                        //        await model.FileNew.CopyToAsync(fileStream);
                        //    }
                        //    model.file = uniquefilename;
                        //}
                    }

                    if (model.FileNew != null)
                    {
                        uploads = Path.Combine(_environment.WebRootPath, "Files");
                        extension = Path.GetExtension(model.FileNew.FileName);
                        basename = Path.Combine(Path.GetDirectoryName(model.FileNew.FileName),
                                Path.GetFileNameWithoutExtension(model.FileNew.FileName));
                        Guid gid = Guid.NewGuid();
                        string uniquefilename = basename + "-" + gid + extension;
                        using (var fileStream = new FileStream(Path.Combine(uploads, uniquefilename), FileMode.Create))
                        {
                            await model.FileNew.CopyToAsync(fileStream);
                        }
                        model.file = uniquefilename;
                    }


                    ApplicationUser user = await _userManager.FindByIdAsync(model.UserId.ToString());
                    if (user != null)
                    {
                        user.Email = model.Email;
                        user.UserName = model.UserName;
                        user.position = model.position;
                        user.skype_id = model.skype_id;
                        user.file = model.file;
                      

                        var result = await _userManager.UpdateAsync(user);
                        _dbContext.SaveChanges();

                        if (result.Succeeded)
                        {
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User Updated Successfully!";
                            RedirectToAction("Index", "Account");
                        }
                        else
                        {
                            TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "User not Updated!";
                        }
                    }
                    else
                    {

                        ModelState.AddModelError(model.UserName, "User Not Found");
                    }
                    switch (submit)
                    {
                        case "Save":
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User updated Successfully.";
                            return RedirectToAction("Index", "Account");

                        case "Save & Continue":
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User updated Successfully.";
                            return RedirectToAction("Register", "Account", new { id = model.UserId });
                    }
                }
                catch (Exception ex)
                {
                    TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                }
                return View(model);
            }
            else
            {
                try
                {
                    //File details fetching
                    //create a place in wwwroot for storing uploaded files
                    if(model.FileNew != null)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "Files");
                        string extension = Path.GetExtension(model.FileNew.FileName);
                        var basename = Path.Combine(Path.GetDirectoryName(model.FileNew.FileName),
                                    Path.GetFileNameWithoutExtension(model.FileNew.FileName));
                        if (model.FileNew != null)
                        {
                            using (var fileStream = new FileStream(Path.Combine(uploads, model.FileNew.FileName), FileMode.Create))
                            {
                                await model.FileNew.CopyToAsync(fileStream);
                            }
                            Guid gid = Guid.NewGuid();
                            string uniquefilename = basename + "-" + gid + extension;
                            model.file = uniquefilename;
                        }
                    }
                    
                    if (ModelState.IsValid)
                    {

                        ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email, position = model.position, skype_id = model.skype_id, file = model.file };
                        user.file = model.file;
                        var result = await _userManager.CreateAsync(user, model.password);

                        if (result.Succeeded)
                        {

                            ViewBag.Succes = true;
                            await _userManager.AddToRoleAsync(user, "Interviewer");
                            user.LockoutEnabled = false;
                            await _dbContext.SaveChangesAsync();
                            return RedirectToAction("Index", "Account");
                        }
                        else
                        {

                            foreach (var error in result.Errors)
                            {
                                ModelState.TryAddModelError(error.Code, error.Description);
                            }
                            return View(model);

                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                return View(model);
            }
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
        ///<summary>
        ///FETCHING JOB POSITION USING CANDIDATE ID FROM DATABASE
        ///</summary>
        ///<param name = "id" ></ param >
        ///< returns ></ returns >
        public string getPositionByCandidateId(int id)
        {
            var job_ID = _dbContext.JobPostCandidate.Where(x => x.candidate_Id == id).FirstOrDefault().job_Id;
            var position = _dbContext.JobPost.AsNoTracking().FirstOrDefault(x => x.ID == job_ID).job_title;
            return position;
        }
        #endregion
    }
}
