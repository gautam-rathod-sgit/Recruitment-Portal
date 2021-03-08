using AutoMapper;
using AutoMapper.QueryableExtensions;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Services
{
    public class CandidatePageService : ICandidatePage
    {
        //getting reference of upper layer repos
        private readonly ICandidateRepository _candidateRepository;
        //getting mapper
        private readonly IMapper _mapper;

        public CandidatePageService(ICandidateRepository candidateRepository, IMapper mapper)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }
        public async Task<CandidateViewModel> AddNewCandidate(CandidateViewModel model)
        {
          var mapped = _mapper.Map<Candidate>(model);
          var newmodel = await _candidateRepository.Add(mapped);
          return _mapper.Map<CandidateViewModel>(newmodel); 
        }

        public async Task<CandidateViewModel> getCandidateById(int id)
        {
            return _mapper.Map<CandidateViewModel>(await _candidateRepository.getById(id));
        }
        //using specification
        public async Task<CandidateViewModel> getCandidateByIdWithSchedules(int cid)
        {
            return _mapper.Map<CandidateViewModel>(await _candidateRepository.getCandidateByIdWithSchedules(cid));
        }

        public async Task<IQueryable<CandidateViewModel>> getCandidates()
        {
            IQueryable<CandidateViewModel> query1;
            var alist = await _candidateRepository.getAll();
            query1 = alist.ProjectTo<CandidateViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task UpdateCandidate(CandidateViewModel model)
        {
            var mapped = _mapper.Map<Candidate>(model);
            await _candidateRepository.Update(mapped);
        }


        public async Task<CandidateViewModel> getCandidateByEmailId(string id)
        {
            return _mapper.Map<CandidateViewModel>(await _candidateRepository.getCandidateByEmailId(id));
        }

        //public async Task DeleteCandidateById(int id)
        //{
        //    await _candidateRepository.Delete(await _candidateRepository.getById(id));

        //}
    }
    
}
