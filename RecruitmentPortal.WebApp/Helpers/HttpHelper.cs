using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Helpers
{
    public static class HttpHelper
    {
        private static IHttpContextAccessor HttpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => HttpContextAccessor.HttpContext;
    }
}
