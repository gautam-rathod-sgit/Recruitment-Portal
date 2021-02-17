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
    public class SchedulesPageService : ISchedulesPage
    {
        //getting reference of upper layer repos
        private readonly ISchedulesRepository _schedulesRepository;
        //getting mapper
        private readonly IMapper _mapper;
        public SchedulesPageService(ISchedulesRepository schedulesRepository, IMapper mapper)
        {
            _schedulesRepository = schedulesRepository;
            _mapper = mapper;
        }
        public async Task<SchedulesViewModel> AddNewSchedules(SchedulesViewModel model)
        {
            var mapped = _mapper.Map<Schedules>(model);
            var result = await _schedulesRepository.Add(mapped);
            var data = _mapper.Map<SchedulesViewModel>(result);
            return data;
        }

        public async Task<SchedulesViewModel> GetSchedulesById(int id)
        {
            return _mapper.Map<SchedulesViewModel>(await _schedulesRepository.getById(id));
        }

        public async Task<IQueryable<SchedulesViewModel>> GetSchedulesByUserId(string uid)
        {


            IQueryable<SchedulesViewModel> query1;
            var item = await _schedulesRepository.getSchedulesByUserId(uid);
            query1 = item.ProjectTo<SchedulesViewModel>(_mapper.ConfigurationProvider);
            return query1;
        }

        public async Task UpdateSchedule(SchedulesViewModel model)
        {
            await _schedulesRepository.Update(_mapper.Map<Schedules>(model));
        }

        public async Task<SchedulesViewModel> getSchedulesByCandidateId(int Cid)
        {
            var item = await _schedulesRepository.getSchedulesByCandidateId(Cid);
            return _mapper.Map<SchedulesViewModel>(item);
        }

        public async Task DeleteSchedule(int id)
        {
            await _schedulesRepository.Delete(await _schedulesRepository.getById(id));
        }
    }
}
