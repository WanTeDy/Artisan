using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Artisan.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Artisan.Controllers
{
    public class AutomaticsController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        

        // GET: /Automatics/
        /*public ActionResult Index()
        {
            ViewBag.Title = "Artisan автоматика";
            return View();
        }*/

        public ActionResult Garage()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Автоматика для гаражных ворот");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", cat.Id);
            }
            return HttpNotFound();
        }

        public ActionResult Sliding()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Автоматика для откатных ворот");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", cat.Id);
            }
            return HttpNotFound();
        }

        public ActionResult Swing()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Автоматика для распашных ворот");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", cat.Id);
            }
            return HttpNotFound();
        }

        public ActionResult Accessories()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Аксессуары");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", cat.Id);
            }
            return HttpNotFound();
        }
    }
}