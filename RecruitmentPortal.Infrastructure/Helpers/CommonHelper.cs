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
        public const string RegexNumber = "([0-9]+)";
        public const string RegexDecimal = "^[0-9]+(\\.[0-9]{1,2})?$";
        public const string RegexText = "^[a-zA-Z][a-zA-Z_ .]*$";
        public const string RegexUsername = "^[a-zA-Z][a-zA-Z0-9_.]*$";
        public const string RegexAlphaNumSpecialChar = "^[a-zA-Z][A-Za-z0-9_ @./#&+-]*$";
        public const string RegexDOB = "[0-9]{2}/[0-9]{2}/[0-9]{4}";
        #endregion

        #region Methods
        public static string UserId { get; set; }
        public static string ConnectionString { get; set; }
        #endregion
    }
}
