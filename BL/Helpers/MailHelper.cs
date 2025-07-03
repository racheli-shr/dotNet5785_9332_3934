using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Helpers
{


    //תוספת אמורה לעבוד רק שכדי לשלוח דרך האימייל שלי צריך סיסמא למייל שלי

    public static class MailHelper
    {
        public static void SendEmail(string to, string subject, string body)
        {
            var fromAddress = new MailAddress("s83934@bnoteli7.com", "Call Management");
            var toAddress = new MailAddress(to);
            const string fromPassword = "חסוי"; // מומלץ לשים בקובץ קונפיג

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
        }
    }



}

