using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Data.Enum
{
   
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
