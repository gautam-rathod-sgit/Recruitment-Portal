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
        #endregion

        #region Constructor
        public AccountController(IPasswordHasher<ApplicationUser> passwordHash,
            RecruitmentPortalDbContext dbContext,
           UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            passwordHasher = passwordHash;
            _emailService = emailService;
            _dbContext = dbContext;
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

        /// <summary>
        /// This method is used for Updating registered User's Data [GET - Fetches User data]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateUser(string id)
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
                }
            }
            return View(model);
        }

        /// <summary>
        ///This method is used for Updating registered User's Data [POST- Save updated data to the Database]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="skype_id"></param>
        /// <param name="position"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUser(ApplicationUserViewModel model, string submit)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId.ToString());

            try
            {
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.position = model.position;
                    user.skype_id = model.skype_id;
                    user.PasswordHash = passwordHasher.HashPassword(user, model.password);

                    var result = await _userManager.UpdateAsync(user);
                    _dbContext.SaveChanges();

                    if (result.Succeeded)
                    {
                        TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User Updated Successfully!";
                        RedirectToAction("Index", "Home");
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
                        return RedirectToAction("UpdateUser", "Account", new { id = model.UserId });
                }
            }
            catch (Exception ex)
            {
                TempData[EnumsHelper.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return View(model);
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
