using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entities;
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
       
        private readonly IDepartmentPage _departmentPageservices;
        //for userid
        private readonly UserManager<ApplicationUser> _userManager;

        //for taking image's property : media stuff
        private readonly IWebHostEnvironment _environment;
        public QualificationController(IDepartmentPage departmentPageservices, IWebHostEnvironment environment, IDegreePage degreePageServices,
             UserManager<ApplicationUser> userManager)
        {
            _degreePageServices = degreePageServices;
            _departmentPageservices = departmentPageservices;
            _environment = environment;
            _userManager = userManager;
        }

        /// <summary>
        /// SHOWING ALL THE DEGREE LIST WITH DEPARTMENTS
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string s)
        {
            try
            {
                if (s != null)
                    ViewData["msg"] = s;
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
        public async Task<IActionResult> DeleteDegree(int id)
        {
            await _degreePageServices.DeleteDegree(id);
            return RedirectToAction("Index", "Qualification");
        }
        //update Degree[GET POST]

        /// <summary>
        /// UPDATE DEGREE [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDegree(int id)
        {
            var category = await _degreePageServices.getDegreeById(id);
            return View(category);
        }

        /// <summary>
        /// UPDATE DEGREE [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateDegree(DegreeViewModel model)
        {
            await _degreePageServices.UpdateDegree(model);
            return RedirectToAction("Index", "Qualification");
        }

        //DEPARTMENT CRUD

        /// <summary>
        /// ADDING DEPARTMENT [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddNewDept(int id)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            model.DegreeId = id;
            return View(model);
        }

        /// <summary>
        /// ADDING DEPARTMENT [POST]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewDept(DepartmentViewModel model)
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
        public async Task<IActionResult> DeleteDept(int id)
        {
            try
            {
                await _departmentPageservices.DeleteDepartment(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index", "Qualification");
        }


        /// <summary>
        /// UPDATE DEPARTMENT [GET]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dept_id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDept(int id, int dept_id)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            try
            {
                model = await _departmentPageservices.getDepartmentById(id);
                model.DegreeId = dept_id;
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
        [HttpPost]
        public async Task<IActionResult> UpdateDept(DepartmentViewModel model)
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
