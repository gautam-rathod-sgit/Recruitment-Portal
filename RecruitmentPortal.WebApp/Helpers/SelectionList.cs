using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Data.Enum;
using RecruitmentPortal.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Helpers
{
    public class SelectionList
    {
        public static List<Degree> GetDegreeList()
        {
            using (RecruitmentPortalDbContext _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.Degree.Where(m => m.isActive).OrderBy(x => x.ID).ToList();
            }
        }
        //public static SelectList GetUserList()
        //{
        //    using (RecruitmentPortalDbContext _dbContext = BaseContext.GetDbContext())
        //    {
        //        var enumData = (from user in _dbContext.Users
        //                        select new
        //                        {
        //                            UserId = user.Id,
        //                            Username = user.UserName
        //                        }).ToList();
        //        return new SelectList(enumData, "UserId", "Username");
        //    }
        //}



        public static SelectList GetReferenceTypeList()
        {
            var enumData = from ReferenceType e in Enum.GetValues(typeof(ReferenceType))
                           select new { ID = (int)e, Name = EnumExtension.DescriptionAttr(e) };
            return new SelectList(enumData, "ID", "Name");
        }
        public static SelectList GetNoticePeriodTypeList()
        {
            var enumData = from NoticePeriodType e in Enum.GetValues(typeof(NoticePeriodType))
                           select new { ID = (int)e, Name = EnumExtension.DescriptionAttr(e) };
            return new SelectList(enumData, "ID", "Name");
        }
        public static SelectList GetRoundTypeList()
        {
            var enumData = from RoundType e in Enum.GetValues(typeof(RoundType))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            return new SelectList(enumData, "ID", "Name");
        }

        public static SelectList GetStatusTypeList()
        {
            var enumData = from StatusType e in Enum.GetValues(typeof(StatusType))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            return new SelectList(enumData, "ID", "Name");
        }
    }
}
