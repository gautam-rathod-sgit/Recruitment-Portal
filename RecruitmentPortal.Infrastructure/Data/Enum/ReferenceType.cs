using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Data.Enum
{
    public enum ReferenceType
    {
        [Description("LinkedIn")]
        LinkedIn = 1,
        [Description("Indeed")]
        Indeed = 2,
        [Description("Naukri.com")]
        Naukri = 3,
        [Description("Monster.com")]
        Monster = 4,
       [Description("Reference")]
        Reference = 5,
        [Description("Other")]
        Other = 6
    }
}
