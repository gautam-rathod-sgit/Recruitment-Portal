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
    public class LoginController : Controller
    {
        #region private variables
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IAuthorize _authorize;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        //for uploading Images/File : media stuff
        private readonly IWebHostEnvironment _environment;


        public IEmailService _emailService { get; }
        #endregion

        #region Constructor
        public LoginController(IWebHostEnvironment environment, SignInManager<ApplicationUser> signInManager,
           ILogger<AccountController> logger,

           UserManager<ApplicationUser> userManager, IEmailService emailService, RecruitmentPortalDbContext dbContext)
        {
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _dbContext = dbContext;
            _emailService = emailService;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This is Login method [GET- Displays empty form]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        /// <summary>
        /// This is Login Method [POST- Send Data to Database(After the form getting filled)]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");
                            var logedInUser = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName && u.EmailConfirmed);
                            TempData[EnumsHelper.NotifyType.Success.GetDescription()] = "User Logged In Successfully..!!";
                            SessionHelper.UserId = logedInUser.Id;
                            SessionHelper.WelcomeUser = logedInUser.UserName;
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("Email", "Invalid login attempt.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Unregistered Email Address");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", Messages.SomethingWrong);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Email", ex.Message);
            }
            return View(model);
        }

        /// <summary>
        /// This method works for Registering the User [GET- Displays empty form]
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Register(string id)
        {
            RegisterViewModel model = new RegisterViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                int actualId = Convert.ToInt32(RSACSPSample.DecodeServerName(id));
                ApplicationUser userObj = await _userManager.FindByIdAsync(actualId.ToString());
            }
            return View(model);
        }

        /// <summary>
        /// This method works for Registering the User [POST- Send Data to Database(After the form getting filled)]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                //File details fetching
                //create a place in wwwroot for storing uploaded files
                var uploads = Path.Combine(_environment.WebRootPath, "Files");
                if (model.FileNew != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, model.FileNew.FileName), FileMode.Create))
                    {
                        await model.FileNew.CopyToAsync(fileStream);
                    }
                    model.file = model.FileNew.FileName;

                }
                if (ModelState.IsValid)
                {

                    ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email, position = model.position, skype_id = model.skype_id, file = model.file };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {

                        ViewBag.Succes = true;
                        await _userManager.AddToRoleAsync(user, "Interviewer");
                        user.LockoutEnabled = false;
                        await _dbContext.SaveChangesAsync();
                        return RedirectToAction("index", "Home");
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
        /// <summary>
        /// This is logout method
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }

        //private void Errors(IdentityResult result)
        //{
        //    foreach (IdentityError error in result.Errors)
        //        ModelState.AddModelError("", error.Description);
        //}

        //forgot password feature
        [HttpGet]
        public IActionResult ForgotThePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotThePassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return RedirectToAction(nameof(ForgotThePasswordConfirmation));
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Login", new { token, email = user.Email }, Request.Scheme);
            //var message = new Message(new string[] { "codemazetest@gmail.com" }, "Reset password token",JToken.Parse(callback),null);

            UserEmailOptions options_new = new UserEmailOptions
            {
                Subject = "Recruitment Portal : Password Reset Token",
                ToEmails = new List<string>() { user.Email },
                Body = callback
            };


            await _emailService.SendTestEmail(options_new);
            return RedirectToAction(nameof(ForgotThePasswordConfirmation));
        }

        public IActionResult ForgotThePasswordConfirmation()
        {
            return View();
        }

        //for reset password
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion
    }
}
