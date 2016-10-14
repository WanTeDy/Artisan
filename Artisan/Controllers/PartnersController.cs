using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Artisan.Controllers
{
    public class PartnersController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Artisan Наши партнёры";
            return View();
        }        
    }
}