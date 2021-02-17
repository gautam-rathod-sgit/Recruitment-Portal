using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Interfaces
{
    public interface IEmailService
    {
          Task SendTestEmail(UserEmailOptions userEmailOptions);
    }
}
