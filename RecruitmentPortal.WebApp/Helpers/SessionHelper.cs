using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Helpers
{
    public class SessionHelper
    {
        public static string UserId
        {
            get => HttpHelper.HttpContext.Session.GetString("UserId") == null ? string.Empty : HttpHelper.HttpContext.Session.GetString("UserId");
            set => HttpHelper.HttpContext.Session.SetString("UserId", value);
        }
        public static string WelcomeUser
        {

            get => HttpHelper.HttpContext.Session.GetString("WelcomeUser") == null ? "Guest" : HttpHelper.HttpContext.Session.GetString("WelcomeUser");
            set => HttpHelper.HttpContext.Session.SetString("WelcomeUser", value);
        }
    }
}
