using Microsoft.AspNetCore.DataProtection;
using RecruitmentPortal.WebApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Helpers
{
    public class EmailHelper
    {
        //For Security: Encrypt URL Params
        private readonly IDataProtector protector;

        public EmailHelper(IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.JobAppDetailIdRouteValue);
        }

        public bool SendEmail(string userEmail, string confirmationLink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("admin@recruitmentportal.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Confirm your email";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationLink;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("admin@recruitmentportal.com", "Pass4Admin");
            client.Host = "smtpout.secureserver.net";
            client.Port = 80;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }
    

}
