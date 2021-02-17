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
    public class SchedulesUsersPageService : ISchedulesUsersPage
    {
        //getting reference of upper layer repos
        private readonly ISchedulesUsersRepository _schedulesUsersRepository;
        //getting mapper
        private readonly IMapper _mapper;
        public SchedulesUsersPageService(ISchedulesUsersRepository schedulesUsersRepository, IMapper mapper)
        {
            _schedulesUsersRepository = schedulesUsersRepository;
            _mapper = mapper;
        }
        public async Task<SchedulesUsersViewModel> AddNewSchedulesUsers(SchedulesUsersViewModel model)
        {
            var mapped = _mapper.Map<SchedulesUsers>(model);
            await _schedulesUsersRepository.Add(mapped);
            return model;
        }

        public async Task DeleteScheduleUsers(int id)
        {
            await _schedulesUsersRepository.Delete(await _schedulesUsersRepository.getById(id));
        }

        public async Task<IQueryable<SchedulesUsersViewModel>> getSchedulesUsers()
        {
            IQueryable<SchedulesUsersViewModel> query1;
            var item = await _schedulesUsersRepository.getAll();
            query1 = item.ProjectTo<SchedulesUsersViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public Task<SchedulesUsersViewModel> getSchedulesUsersId(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSchedulesUsers(SchedulesUsersViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
