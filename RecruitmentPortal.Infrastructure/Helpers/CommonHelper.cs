using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Helpers
{
    public class CommonHelper
    {
        #region Contants
        public const string RegexEmail = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}";
        public const string RegExTelephone = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
        public const string PleaseSelect = "---Select---";
        public const string DateFormat = "dd/MM/yyyy";
        public const string TimeFormate = "HH:mm";
        #endregion

        #region Methods
        public static string ConnectionString { get; set; }
        #endregion
    }
}
