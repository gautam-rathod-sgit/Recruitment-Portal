using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Repository
{
    public class BaseContext
    {
        public static RecruitmentPortalDbContext GetDbContext()
        {
            var dbOptions = new DbContextOptionsBuilder<RecruitmentPortalDbContext>();
            dbOptions.UseSqlServer(CommonHelper.ConnectionString);
            var dbContext = new RecruitmentPortalDbContext(dbOptions.Options);
            return dbContext;
        }
    }
    public class CustomRepository
    {
    }
}
