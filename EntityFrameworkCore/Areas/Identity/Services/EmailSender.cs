using EntityFrameworkCore.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace EntityFrameworkCore.Areas.Identity.Services
{
    public class EmailSender:IEmailSender
    {
        public async Task SendEmailAsync(string email,string subject,string message)
        {
            using (var Client = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName="fathimohamad7575",
                    Password= "nnfh idgo civs fnbm",
                };
                Client.Credentials = Credential;
                Client.Host = "smtp.gmail.com";
                Client.Port = 587;
                Client.EnableSsl = true;

                using(var EmailMessage=new MailMessage())
                {
                    EmailMessage.To.Add(new MailAddress(email));
                    EmailMessage.From = new MailAddress("fathimohamad7575@gmail.com");
                    EmailMessage.Subject = subject;
                    EmailMessage.IsBodyHtml = true;
                    EmailMessage.Body = message;
                    
                    Client.Send(EmailMessage);
                }
                await Task.CompletedTask;
            }
        }
    }
}
