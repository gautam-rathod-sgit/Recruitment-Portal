using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class QualificationController : BaseController
    {
        #region private variables

        IQueryable<DegreeViewModel> degreeList;
        private readonly IDegreePage _degreePageServices;
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);

        private readonly IDepartmentPage _departmentPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RecruitmentPortalDbContext _dbContext;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region constructor
        public QualificationController(IDepartmentPage departmentPageservices,
         IWebHostEnvironment environment,
         IDegreePage degreePageServices,
          RecruitmentPortalDbContext dbContext,
          UserManager<ApplicationUser> userManager)
        {
            _degreePageServices = degreePageServices;
            _departmentPageservices = departmentPageservices;
            _environment = environment;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        #endregion

        #region public methods

        /// <summary>
        /// SHOWING ALL THE DEGREE LIST WITH DEPARTMENTS
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// FOR FETCHING ALL DEGREES 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="activeCandidate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetAllDegrees(string s, string activeCandidate)
        {
            IQueryable<DegreeViewModel> degreeList;
            List<DegreeViewModel> newlist = new List<DegreeViewModel>();

            try
            {
                if (s != null)
                    ViewData["msg"] = s;
                if (activeCandidate != null)
                {
                    ViewBag.active = activeCandidate;
                }

                degreeList = await _degreePageServices.getDegrees();
                newlist = degreeList.ToList();
                foreach (var item in newlist)
                {
                    item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                    item.Departments.ToList();
                }

                return Json(new { data = newlist });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = newlist });
            }
        }

        /// <summary>
        /// ADDING NEW DEGREE [GET]
        /// </summary>
        /// <returns></returns>
        public IActionResult AddNewDegree()
        {
            return View();
        }
        /// <summary>
        /// ADDING NEW DEGREE [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewDegree(DegreeViewModel model)
        {
            try
            {
                degreeList = await _degreePageServices.getDegrees();
                foreach (var item in degreeList)
                {
                    if (item.degree_name == model.degree_name)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Degree already exists !!";
                        return View("Index");
                    }
                }


                if (model.ID != 0)
                {
                    return RedirectToAction("UpdateDegreePost", model);
                    //await _jobCategoryPageservices.UpdateCategory(model);
                }
                else
                {
                    model.isActive = true;
                    await _degreePageServices.AddNewDegree(model);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index", "Qualification");
        }

        //Delete Degree [GET]
        /// <summary>
        /// DELETE DEGREE [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteDegree(string id)
        {
            await _degreePageServices.DeleteDegree(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
            return RedirectToAction("Index", "Qualification");
        }
        //update Degree[GET POST]

        /// <summary>
        /// UPDATE DEGREE [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UpdateDegree(string id, bool status, bool editMode)
        {
            DegreeViewModel model = new DegreeViewModel();
            try
            {
                model = await _degreePageServices.getDegreeById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                if (status)
                {
                    bool isActiveCandidate = false;
                    //checking if any candidate is active with this degree
                    List<JobApplications> jobAppList = new List<JobApplications>();
                    var listofCandidates = _dbContext.Candidate.Where(x => x.degree.Contains(model.degree_name)).ToList();
                    foreach (var item in listofCandidates)
                    {
                        var data = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).FirstOrDefault();
                        if (data != null)
                            jobAppList.Add(data);
                    }
                    var temp = jobAppList.Where(x => x.status == status_Pending).Any();
                    if (temp)
                    {
                        isActiveCandidate = true;
                    }
                    if (isActiveCandidate)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Action Denied .Candidate is Active with items to be Deleted !!";
                        return RedirectToAction("Index", "Qualification");
                    }
                    else
                    {
                        //deactivating Degree
                        model.isActive = false;
                        //deactivating department

                        using (_dbContext)
                        {
                            _dbContext.Department
                            .Where(x => x.DegreeId == model.ID)
                            .ToList()
                            .ForEach(a =>
                            {
                                a.isActive = false;
                            }
                            );
                            _dbContext.SaveChanges();
                        }
                        return RedirectToAction("UpdateDegreePost", "Qualification", model);
                    }
                }
                else
                {
                    if (editMode)
                    {
                        return View(model);
                    }
                    else
                    {
                        model.isActive = true;
                        return RedirectToAction("UpdateDegreePost", "Qualification", model);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return View(model);
        }

        /// <summary>
        /// UPDATE DEGREE [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDegreePost(DegreeViewModel model)
        {
            try
            {
                await _degreePageServices.UpdateDegree(model);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index", "Qualification");
        }

        /// <summary>
        /// THIS METHOD IS USED TO FETCH DETAILS OF PARTICULAR Degree
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            DegreeViewModel model = new DegreeViewModel();
            model.EncryptedId = id;

            return View(model);
        }
       
         public async Task<IActionResult> GetDegreeDetails(string id)
        {
            List<DepartmentViewModel> itemList = new List<DepartmentViewModel>();
            DegreeViewModel item = new DegreeViewModel();

            try
            {
                item = await _degreePageServices.GetDegreeWithDepartmentById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                item.EncryptedId = RSACSPSample.EncodeServerName((item.ID).ToString());
                foreach (var obj in item.Departments)
                {
                    obj.EncryptedId = RSACSPSample.EncodeServerName((obj.ID).ToString());
                    obj.EncryptedDegreeId = item.EncryptedId;
                }
                itemList = item.Departments;
                return Json(new { data = itemList });
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
                return Json(new { data = itemList });
            }
        }


        //DEPARTMENT CRUD


        /// <summary>
        /// ADDING DEPARTMENT [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewDeptPost(DepartmentViewModel model)
        {
            try
            {
                
                DegreeViewModel degrees = await _degreePageServices.GetDegreeWithDepartmentById(model.DegreeId);
                foreach (var item in degrees.Departments)
                {
                    if (item.dept_name == model.dept_name)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Department already exists !!";
                        return RedirectToAction("Index", "Qualification");
                    }
                }
                if (model.ID != 0)
                {
                    return RedirectToAction("UpdateDeptPost", model);
                    //await _jobCategoryPageservices.UpdateCategory(model);
                }
                else
                {
                    model.isActive = true;
                    await _departmentPageservices.AddNewDepartment(model);
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index", "Qualification");
        }


        /// <summary>
        /// DELETE DEPARTMENT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public async Task<IActionResult> DeleteDept(string id)
        //{
        //    try
        //    {
        //        await _departmentPageservices.DeleteDepartment(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return RedirectToAction("Index", "Qualification");
        //}



        /// <summary>
        /// UPDATE DEPARTMENT [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dept_id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDept(string id, string degreeID, bool status, bool editMode)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            try
            {
                model = await _departmentPageservices.getDepartmentById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                //model.DegreeId = Convert.ToInt32(RSACSPSample.DecodeServerName(degree_id));

                if (status)
                {
                    bool isActiveCandidate = false;
                    //checking if any candidate is active with this degree
                    List<JobApplications> jobAppList = new List<JobApplications>();
                    var listofCandidates = _dbContext.Candidate.Where(x => x.degree.Contains(model.dept_name)).ToList();
                    foreach (var item in listofCandidates)
                    {
                        var data = _dbContext.jobApplications.Where(x => x.candidateId == item.ID).FirstOrDefault();
                        if (data != null)
                            jobAppList.Add(data);
                    }
                    var temp = jobAppList.Where(x => x.status == status_Pending).Any();
                    if (temp)
                    {
                        isActiveCandidate = true;
                    }
                    if (isActiveCandidate)
                    {
                        TempData[EnumsHelper.NotifyType.Error.GetDescription()] = "Action Denied .Candidate is Active with items to be Deleted !!";
                        return RedirectToAction("Details", "Qualification", new { id = degreeID }); 

                    }
                    else
                    {
                        model.isActive = false;
                        return RedirectToAction("UpdateDeptPost", "Qualification", model);
                    }
                }
                else
                {
                    if (editMode)
                    {
                        return View(model);
                    }
                    else
                    {
                        model.isActive = true;
                        return RedirectToAction("UpdateDeptPost", "Qualification", model);
                    }

                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
            return View(model);
        }


        /// <summary>
        /// UPDATE DEPARTMENT [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDeptPost(DepartmentViewModel model)
        {
            try
            {
                    await _departmentPageservices.UpdateDepartment(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Details", "Qualification",new { id = RSACSPSample.EncodeServerName((model.DegreeId).ToString())});
        }

        /// <summary>
        /// for Loading the popup for adding or edititng the Degree.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RenderDegreeView(string id)
        {
            DegreeViewModel model = new DegreeViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                model = await _degreePageServices.getDegreeById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
            }
            return PartialView("_DegreeView", model);
        }
        /// <summary>
        /// for Loading the popup for adding or edititng the Department.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RenderDeptView(string id, bool isParentCall)
        {
            
            DepartmentViewModel model = new DepartmentViewModel();
            if (isParentCall)
            {
                model.DegreeId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            }
            else
            {
                if (!string.IsNullOrEmpty(id))
                {
                    model = await _departmentPageservices.getDepartmentById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                    model.EncryptedDegreeId = RSACSPSample.EncodeServerName((model.DegreeId).ToString());
                }
            }
            
            return PartialView("_DepartmentView", model);
        }
        #endregion

    }
}
