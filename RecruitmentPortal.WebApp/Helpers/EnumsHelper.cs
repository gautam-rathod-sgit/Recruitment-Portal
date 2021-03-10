using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Helpers
{
    public class EnumsHelper
    {
        public enum NotifyType
        {
            [Description("Success Message")]
            Success = 1,

            [Description("Error Message")]
            Error = 2,

            [Description("Warning Message")]
            WarningMessage = 3,

            [Description("System Error Message")]
            SystemErrorMessage = 4
        }
    }

    public static class EnumExtension
    {
        /// <summary>
        /// The get description.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDescription(this Enum element)
        {
            var type = element.GetType();
            var memberInfo = type.GetMember(Convert.ToString(element));
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            return Convert.ToString(element);
        }
    }
}
