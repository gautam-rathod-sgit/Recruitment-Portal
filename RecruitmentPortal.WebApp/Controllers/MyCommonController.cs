using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Controllers
{
    public  class MyCommonController : Controller
    {
        public static List<Department> GetDept(int Id)
        {
         
            using (RecruitmentPortalDbContext _dbContext = BaseContext.GetDbContext())
            {
                //List<Department> DepartmentList = new List<Department>();

                //Getting data from database Using EntityFramework Core
                return _dbContext.Department.Where(a => a.DegreeId == Id && a.isActive)/*Select(m => new { Id = m.ID, Name = m.dept_name })*/.ToList();

                //if (DepartmentList != null)
                //{
                //    //Inserting Select item in List
                //    DepartmentList.Insert(0, new Department { ID = 0, dept_name = "Select Department" });

                //}
                //return DepartmentList;
            }

        }
    }
}
