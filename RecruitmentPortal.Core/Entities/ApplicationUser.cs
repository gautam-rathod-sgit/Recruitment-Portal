using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string position { get; set; }
        public string skype_id { get; set; }
        //relationship with SchedulesUsers
        public IList<SchedulesUsers> SchedulesUsers { get; set; }
    }
}
