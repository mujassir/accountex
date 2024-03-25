using System.Text;
using System.Net.Mail;
using System.IO;

namespace BussinessLogic
{


    public static class EmailManager
    {

        public static void SendSalaryProcessEmail()
        {
            //var html = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Template/Email.html"));
            //var baseurl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];

            //var subject = "Salary Process Request";
            //html = html.Replace("[BaseUrl]", baseurl);
            //html = html.Replace("[FirstName]", input.FirstName);
            //html = html.Replace("[LastName]", input.LastName);
            //SendEmail(input.Email, subject, html);


        }
        public static void SendEmail(string to, string subject, string body, string attachment)
        {

            var message = new MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);
            if (!string.IsNullOrWhiteSpace(attachment))
                message.Attachments.Add(new Attachment(attachment));
            using (var smtp = new SmtpClient())
            {

                smtp.Send(message);
            }
        }
        //private string GetHtml(EmailType type)
        //{
        //    return "";

        //}
    }
}
