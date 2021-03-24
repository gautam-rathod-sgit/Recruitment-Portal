using Microsoft.Extensions.Options;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly SMTPConfigModel _smtpConfig;
        public EmailService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }
        //this method is used because SendEmail is already a method inside EmailService of MailKit
        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            await SendEmail(userEmailOptions);
        }

        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            //prepare mail item
            MailMessage mail = new MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };
            //add reciever mails
            foreach (var item in userEmailOptions.ToEmails)
            {
                mail.To.Add(item);
            }

            //provide username and password to network
            NetworkCredential networkCredentials = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

            //create instance of smtp
            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = false,
                UseDefaultCredentials = _smtpConfig.UserDefaultCredentials,
                Credentials = networkCredentials
            };
            //provide mail encoding
            mail.BodyEncoding = Encoding.Default;

            //finally send mail
            await smtpClient.SendMailAsync(mail); 
        }
    }
}
