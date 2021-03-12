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

namespace RecruitmentPortal.WebApp.Controllers
{
    public class QualificationController : Controller
    {
        IQueryable<DegreeViewModel> degreeList;
        private readonly IDegreePage _degreePageServices;
        private string status_Pending = Enum.GetName(typeof(JobApplicationStatus), 1);

        private readonly IDepartmentPage _departmentPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RecruitmentPortalDbContext _dbContext;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
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

        /// <summary>
        /// SHOWING ALL THE DEGREE LIST WITH DEPARTMENTS
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string s, string activeCandidate)
        {
            try
            {
                if (s != null)
                    ViewData["msg"] = s;
                if (activeCandidate != null)
                {
                    ViewBag.active = activeCandidate;
                }
                degreeList = await _degreePageServices.GetAllDegreeWithDepartment();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return View(degreeList);
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
                        ViewData["msg"] = model.degree_name;
                        return View();
                    }
                }
                model.isActive = true;
                await _degreePageServices.AddNewDegree(model);
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
        public async Task<IActionResult> UpdateDegree(string id, bool deactivate, bool editMode)
        {
            DegreeViewModel model = new DegreeViewModel();
            try
            {
                model = await _degreePageServices.getDegreeById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                if (deactivate)
                {
                    bool isActiveCandidate = false;
                    //checking if any candidate is active with this degree
                    List<JobApplications> jobAppList = new List<JobApplications>();
                    var listofCandidates = _dbContext.Candidate.Where(x => x.degree.Contains(model.degree_name)).ToList();
                    foreach(var item in listofCandidates)
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
                        TempData["deactivate"] = "Deactivation Failed !! Candidate with Degree is Active";
                        return RedirectToAction("Index", "Qualification", new { activeCandidate = TempData["deactivate"]});
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

        //DEPARTMENT CRUD

        /// <summary>
        /// ADDING DEPARTMENT [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddNewDept(string id)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            try
            {
                model.DegreeId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return View(model);
        }

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
                        TempData["msg"] = model.dept_name;
                        return RedirectToAction("Index", "Qualification", new { s = TempData["msg"] });
                    }
                }
                model.isActive = true;
                await _departmentPageservices.AddNewDepartment(model);
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
        public async Task<IActionResult> UpdateDept(string id, string degree_id, bool deactivated, bool editMode)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            try
            {
                model = await _departmentPageservices.getDepartmentById(Convert.ToInt32(RSACSPSample.DecodeServerName(id)));
                //model.DegreeId = Convert.ToInt32(RSACSPSample.DecodeServerName(degree_id));

                if (deactivated)
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
                        TempData["deactivate"] = "Deactivation Failed !! Candidate with Department is Active";
                        return RedirectToAction("Index", "Qualification", new { activeCandidate = TempData["deactivate"] });
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
            
            return RedirectToAction("Index", "Qualification");
        }

    }
}
