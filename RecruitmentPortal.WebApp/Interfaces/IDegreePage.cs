using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IDegreePage
    {
        public Task<IQueryable<DegreeViewModel>> getDegrees();
        public Task<DegreeViewModel> getDegreeById(int id);
        public Task<DegreeViewModel> AddNewDegree(DegreeViewModel model);
        public Task DeleteDegree(int id);
        public Task UpdateDegree(DegreeViewModel model);

        //using specification

        //for degrees with all departments
        public Task<DegreeViewModel> GetDegreeWithDepartmentById(int id);
        //for all degrees with all departments
        public Task<IQueryable<DegreeViewModel>> GetAllDegreeWithDepartment();

    }
}
