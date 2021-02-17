using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IDepartmentPage
    {
        public Task<IQueryable<DepartmentViewModel>> getDepartment();
        public Task<DepartmentViewModel> getDepartmentById(int id);
        public Task<DepartmentViewModel> AddNewDepartment(DepartmentViewModel model);
        public Task DeleteDepartment(int id);
        public Task UpdateDepartment(DepartmentViewModel model);
    }
}

