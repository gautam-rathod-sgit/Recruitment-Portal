using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Repository
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        public Task<Candidate> getCandidateByEmailId(string id);
        public Task<Candidate> getCandidateByIdWithSchedules(int id);
    }
}
