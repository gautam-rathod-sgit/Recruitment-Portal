using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.Infrastructure.Repository;
using RecruitmentPortal.WebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    //[Authorize]
    public class BaseController : Controller
    {
        #region Public Methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                #region Handle Session Time Out
                if (!string.IsNullOrEmpty(SessionHelper.UserId))
                {
                    SetSessionValues(filterContext);
                }
                //else
                //{
                //    RedirectToLoginPage(filterContext);
                //}
                #endregion
            }
            else
            {
                RedirectToLoginPage(filterContext);
            }
        }
        #endregion

        #region Private Methods
        private void RedirectToLoginPage(ActionExecutingContext filterContext)
        {
            var url = new UrlHelper(filterContext);
            var loginUrl = url.Action("Login", "Login");
            if (loginUrl != null)
            {
                HttpContext.Session.Clear();
                HttpContext.SignOutAsync();
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
        }

        private void SetSessionValues(ActionExecutingContext filterContext)
        {
            var loggedInUser = HttpContext.User;
            var loggedInUserName = loggedInUser.Identity.Name;

            string userName = loggedInUserName;

            var logedInUser = BaseContext.GetDbContext().Users.FirstOrDefault(u => u.UserName == userName && u.EmailConfirmed);

            if (logedInUser == null)
            {
                RedirectToLoginPage(filterContext);
            }
            else
            {
                SessionHelper.UserId = logedInUser.Id;
                SessionHelper.WelcomeUser = logedInUser.UserName;
                CommonHelper.UserId = logedInUser.Id;
            }
        }
        #endregion
    }
}
