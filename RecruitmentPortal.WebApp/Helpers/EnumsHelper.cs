using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        public enum NoticePeriodType
        {
            [Description("Immediate")]
            Immediate,
            [Description("15 Days")]
            day15 = 15,
            [Description("30 Days")]
            day30 = 30,
            [Description("60 Days")]
            day60 = 60,
            [Description("90 Days")]
            day90 = 90,
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
        public static string DescriptionAttr<T>(this T source)
        {
           
                FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }

}
