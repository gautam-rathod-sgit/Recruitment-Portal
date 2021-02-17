using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Specification.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Specification
{
    public class CandidateWithSchedulesSpecification : BaseSpecification<Candidate>
    {
        public CandidateWithSchedulesSpecification()
        : base(null)
        {
            AddInclude(p => p.Schedules);
        }
        public CandidateWithSchedulesSpecification(int id)
          : base(p => p.ID == id)
        {
           AddInclude(p => p.Schedules);
        }
    }
}
