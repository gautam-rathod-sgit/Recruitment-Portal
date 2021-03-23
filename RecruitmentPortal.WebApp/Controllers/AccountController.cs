using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
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
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IAuthorize _authorize;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public IEmailService _emailService { get; }
        private readonly RecruitmentPortalDbContext _dbContext;
        //for uploading Images/File : media stuff
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Constructor
        public AccountController(IPasswordHasher<ApplicationUser> passwordHash,
            RecruitmentPortalDbContext dbContext, IWebHostEnvironment environment,
           UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            passwordHasher = passwordHash;
            _emailService = emailService;
            _dbContext = dbContext;
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
                if (user != null)
                {
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
        #endregion
    }
}
