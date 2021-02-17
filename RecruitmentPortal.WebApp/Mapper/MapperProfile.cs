using AutoMapper;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<JobCategory, JobCategoryViewModel>().ReverseMap();
            CreateMap<JobPost,JobPostViewModel>().ReverseMap();
            CreateMap<Degree, DegreeViewModel>().ReverseMap();
            CreateMap<Department, DepartmentViewModel>().ReverseMap();
            CreateMap<Candidate, CandidateViewModel>().ReverseMap();
            CreateMap<Schedules, SchedulesViewModel>().ReverseMap();
            CreateMap<JobApplications, JobApplicationViewModel>().ReverseMap();
            CreateMap<JobPostCandidate, JobPostCandidateViewModel>().ReverseMap();
            CreateMap<SchedulesUsers, SchedulesUsersViewModel>().ReverseMap();

        }
    }
}
