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

        //For uploading file
        public string file { get; set; }
        public IList<SchedulesUsers> SchedulesUsers { get; set; }
    }
}
