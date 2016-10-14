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
    public class GatesController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        //
        // GET: /Gates/
        /*public ActionResult Index()
        {
            ViewBag.Title = "Artisan ворота";
            return View();
        }*/

        public ActionResult Garage()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Гаражные ворота");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", new { id = cat.Id });
            }
            return HttpNotFound();
        }

        public ActionResult Sliding()
        {            
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Откатные ворота");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", new { id = cat.Id });
            }
            return HttpNotFound();
        }

        public ActionResult Swing()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Распашные ворота");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", new { id = cat.Id });
            }
            return HttpNotFound();
        }
    }
}