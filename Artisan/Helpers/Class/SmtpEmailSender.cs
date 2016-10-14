using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Mail;

namespace Artisan.Helpers.Class
{
    public class SmtpEmailSender : IEmailSender
    {        
        public SmtpEmailSender(string smtp, string from, string password, int port = 25)
        {
            SmtpServer = smtp;
            Sender = from;
            Password = password;
            Port = port;
        }

        public string SmtpServer { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }


        public Task Send(string destination, string subject, string body)
        {
            // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient client = new SmtpClient(SmtpServer, Port);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(Sender, Password);
            client.EnableSsl = true;

            // создаем письмо: message.Destination - адрес получателя
            var mail = new MailMessage(Sender, destination);
            mail.Subject = subject;
            mail.Body = HttpUtility.HtmlEncode(body);
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);
        }
    }
}