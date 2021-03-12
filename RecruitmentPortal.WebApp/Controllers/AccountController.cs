using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IAuthorize _authorize;
        private readonly RecruitmentPortalDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private IPasswordHasher<ApplicationUser> passwordHasher;
       

        public AccountController(SignInManager<ApplicationUser> signInManager,
           ILogger<AccountController> logger,
           IPasswordHasher<ApplicationUser> passwordHash,

           UserManager<ApplicationUser> userManager, IEmailService emailService, RoleManager<IdentityRole> roleManager, RecruitmentPortalDbContext dbContext)
        {
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _dbContext = dbContext;
            passwordHasher = passwordHash;
            _emailService = emailService;
        }


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
        /// This is Login method [GET- Displays empty form]
        /// </summary>
        /// <returns></returns>
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
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(model);
        }

        /// <summary>
        /// This method works for Registering the User [GET- Displays empty form]
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
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
                   
                    ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email, position = model.position, skype_id = model.skype_id , file = model.file};
                                        

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
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
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
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        /// <summary>
        /// This method deletes User record from Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
    
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
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
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

    }
}
