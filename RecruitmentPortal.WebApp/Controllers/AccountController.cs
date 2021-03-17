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
        #endregion

        #region Constructor
        public AccountController(IPasswordHasher<ApplicationUser> passwordHash,
           UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            passwordHasher = passwordHash;
            _emailService = emailService;
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
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
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
        public async Task<IActionResult> UpdateUser(string id, string email, string password, string skype_id, string position, string UserName)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            try
            {
                if (user != null)
                {

                    //checks for null
                    if (!string.IsNullOrEmpty(email))
                        user.Email = email;
                    else
                        ModelState.AddModelError("", "Email cannot be empty");

                    if (!string.IsNullOrEmpty(password))
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    else
                        ModelState.AddModelError("", "Password cannot be empty");
                    if (!string.IsNullOrEmpty(UserName))
                        user.UserName = UserName;
                    else
                        ModelState.AddModelError("", "UserName cannot be empty");
                    if (!string.IsNullOrEmpty(position))
                        user.position = position;
                    else
                        ModelState.AddModelError("", "position cannot be empty");
                    if (!string.IsNullOrEmpty(skype_id))
                        user.skype_id = skype_id;
                    else
                        ModelState.AddModelError("", "skype_id cannot be empty");



                    //updating user
                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                    {
                        IdentityResult result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                            return RedirectToAction("Index");
                        else
                            Errors(result);
                    }
                }
                else
                    ModelState.AddModelError("", "User Not Found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(user);
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
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
                else
                    ModelState.AddModelError("", "User Not Found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View("Index", _userManager.Users);
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
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
