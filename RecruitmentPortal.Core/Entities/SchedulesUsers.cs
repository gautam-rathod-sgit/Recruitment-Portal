using RecruitmentPortal.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecruitmentPortal.Core.Entities
{
    public class SchedulesUsers : Entity
    {
        [ForeignKey("Schedules")]
        public int scheduleId { get; set; }
        public Schedules Schedules { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
