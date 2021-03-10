using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Data.Enum
{
    public enum RoundType
    {
        [Description("Aptitude")]
        Aptitude = 1,
        [Description("Technical")]
        Technical = 2,
        [Description("Practical")]
        Practical = 3,
        [Description("Final Selection")]
        FinalSelection = 4
    }
}
