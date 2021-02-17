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
    public class DepartmentPageService : IDepartmentPage
    {
        //getting reference of upper layer repos
        private readonly IDepartmentRepository _departmentRepository;
        //getting mapper
        private readonly IMapper _mapper;
        public DepartmentPageService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }


        public async Task<DepartmentViewModel> AddNewDepartment(DepartmentViewModel model)
        {
            var mapped = _mapper.Map<Department>(model);
            await _departmentRepository.Add(mapped);
            return model;
        }
    

        public async Task DeleteDepartment(int id)
        {
            await _departmentRepository.Delete(await _departmentRepository.getById(id));
        }

        public async Task<IQueryable<DepartmentViewModel>> getDepartment()
        {
            IQueryable<DepartmentViewModel> query1;
            var item = await _departmentRepository.getAll();
            query1 = item.ProjectTo<DepartmentViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task<DepartmentViewModel> getDepartmentById(int id)
        {
            return _mapper.Map < DepartmentViewModel > (await _departmentRepository.getById(id));
        }

        public async Task UpdateDepartment(DepartmentViewModel model)
        {
            await _departmentRepository.Update(_mapper.Map<Department>(model));
        }
    }
}
