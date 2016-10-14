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
    public class BarriersController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        //
        // GET: /Barriers/
        public ActionResult Index()
        {
            ViewBag.Title = "Artisan шлагбаумы";
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Шлагбаумы");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", new { id = cat.Id } );
            }
            return HttpNotFound();
        }
	}
}