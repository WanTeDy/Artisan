using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Artisan.Models;
using Artisan.Helpers.Class;
using Artisan.Helpers;

namespace Artisan.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Email/
        public ActionResult Index()
        {
            ViewBag.Title = "Artisan обратный звонок";
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public async Task<ActionResult> Index(Mail mail)
        {
            if (ModelState.IsValid)
            {
                IEmailSender mailSender = new SmtpEmailSender(Properties.Settings.Default.SmtpServer, Properties.Settings.Default.EmailFrom, Properties.Settings.Default.EmailPassword);
                string body = mail.Name + "\n" + mail.EmailTo + "\n" + mail.Telephone + "\n" + mail.Message;
                ViewBag.Method = "Email";
                ViewBag.Desc = "Обратная связь";
                try
                {
                    await mailSender.Send("wanted.kaiser@bigmir.net", "Письмо от Artisan", body);
                }
                catch
                {
                    ViewBag.Title = "Artisan ошибка отправления";
                    ViewBag.Result = "Ошибка отправления. Попробуйте позже";
                    return View("Sended");
                }
                ArtisanContext ac = new ArtisanContext();
                Mail newMail = new Mail
                {
                    EmailTo = mail.EmailTo,
                    Message = mail.Message,
                    Name = mail.Name,
                    Telephone = mail.Telephone,
                    Date = DateTime.Now
                };
                ac.Mails.Add(newMail);
                ac.SaveChanges();
                ViewBag.Title = "Artisan письмо отправлено";
                ViewBag.Result = "Ваше письмо отправлено. Мы с Вами свяжемся в ближайшее время";
                return View("Sended");
            }
            ViewBag.Title = "Artisan обратный звонок";
            return View(mail);
        }
    }
}