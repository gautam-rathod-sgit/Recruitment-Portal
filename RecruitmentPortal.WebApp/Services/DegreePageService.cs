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
    public class DegreePageService : IDegreePage
    {
        //getting reference of upper layer repos
        private readonly IDegreeRepository _degreeRepository;
        //getting mapper
        private readonly IMapper _mapper;

        public DegreePageService(IDegreeRepository degreeRepository, IMapper mapper)
        {
            _degreeRepository = degreeRepository;
            _mapper = mapper;
        }
        public async Task<DegreeViewModel> AddNewDegree(DegreeViewModel model)
        {
            var mapped = _mapper.Map<Degree>(model);
            await _degreeRepository.Add(mapped);
            return model;
        }

        public async Task DeleteDegree(int id)
        {
            await _degreeRepository.Delete(await _degreeRepository.getById(id));
        }

        public async Task<IQueryable<DegreeViewModel>> GetAllDegreeWithDepartment()
        {
            IQueryable<DegreeViewModel> query;
            var alist = await _degreeRepository.getAllDegreeWithDepartment();
            query = alist.ProjectTo<DegreeViewModel>(_mapper.ConfigurationProvider);
            return query;
        }

        public async Task<DegreeViewModel> getDegreeById(int id)
        {
            var mapped = _mapper.Map<DegreeViewModel>(await _degreeRepository.getById(id));
            return mapped;
        }

        public async Task<IQueryable<DegreeViewModel>> getDegrees()
        {
            IQueryable<DegreeViewModel> query1;
            var alist = await _degreeRepository.getAll();
            query1 = alist.ProjectTo<DegreeViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task<DegreeViewModel> GetDegreeWithDepartmentById(int id)
        {
            var alist = await _degreeRepository.getDegreeWithDeptById(id);
            var mapped = _mapper.Map<DegreeViewModel>(alist);
            return mapped;
        }

        public async Task UpdateDegree(DegreeViewModel model)
        {
            var mapped = _mapper.Map<Degree>(model);
            await _degreeRepository.Update(mapped);
        }
    }
}
