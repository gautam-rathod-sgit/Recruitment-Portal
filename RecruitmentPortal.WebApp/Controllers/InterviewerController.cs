using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class InterviewerController : Controller
    {
        private string time_format = "HH:mm tt";
        private int reqValue = Convert.ToInt32(Enum.Parse(typeof(StatusType), "Completed"));
        private IQueryable<SchedulesViewModel> slist;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly ISchedulesPage _schedulesPage;
        private readonly UserManager<ApplicationUser> _userManager;
        public InterviewerController(UserManager<ApplicationUser> userManager, RecruitmentPortalDbContext dbContext, ISchedulesPage schedulesPage)
        {          
            _dbContext = dbContext;
            _userManager = userManager;
            _schedulesPage = schedulesPage;
        }

        /// <summary>
        /// SHOWING ALL THE PENDING SCHEDULES FOR INTERVIEWER 
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> Index(string SearchString)
        //{
        //    slist = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));
            
        //    try
        //    {
        //        //filtering the schedules for getting only the incompleted schedules
        //        slist = slist.Where(x => x.status != reqValue);
        //        //Added search box test
        //        if (!String.IsNullOrEmpty(SearchString))
        //        {
        //            slist = slist.Where(s => s.position.ToUpper().Contains(SearchString.ToUpper()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return View(slist);
        //}


        /// <summary>
        /// SHOWING ALL THE COMPLETED SCHEDULES FOR INTERVIEWER 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexCompleted()
        {
            slist = await _schedulesPage.GetSchedulesByUserId(_userManager.GetUserId(HttpContext.User));

            try
            {
                //filtering the schedules for getting only the incompleted schedules
                slist = slist.Where(x => x.status == reqValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(slist);
        }

        /// <summary>
        /// ENLIST ALL THE DETAILS OF PARTICULAR SCHEDULE FOR CANDIDATE TO INTERVIEWER
        /// </summary>
        /// <param name="Sid"></param>
        /// <returns></returns>
        public async Task<IActionResult> ScheduleDetails(int scheduleId)
        {
            //getting schedule by ID
            SchedulesViewModel model = await _schedulesPage.GetSchedulesById(scheduleId);

            try
            {
                //assigning values to some fields
                model.time = model.datetime.ToString(time_format);
                model.statusName = Enum.GetName(typeof(StatusType), model.status);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return View(model);
        }

        //update schedule
        /// <summary>
        /// ACTION WILL UPDATE THE STATUS OF SCHEDULE WITH REMARKS, STATUS, RATING GIVEN BY INTERVIEWER.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Update_Schedule_GET(int id)
        {
            SchedulesViewModel model = new SchedulesViewModel();
            try
            {
                model = await _schedulesPage.GetSchedulesById(id);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }         
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update_Schedule_POST(SchedulesViewModel model)
        {
            try
            {
                //converting enum values to int
                model.status = Convert.ToInt32(model.statusvalue);

                await _schedulesPage.UpdateSchedule(model);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("ScheduleDetails",new {scheduleId = model.ID });
        }
    }
}
