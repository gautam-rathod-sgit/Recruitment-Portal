using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Helpers
{
    public class CommonHelper
    {
        #region Contants
        public const string RegexEmail = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}";
        #endregion

        #region Methods
        public static string ConnectionString { get; set; }
        #endregion
    }
}
