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
    public class ImplementsController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        //
        // GET: /Implements/
        public ActionResult Index()
        {
            ArtisanContext ac = new ArtisanContext();
            Category cat = ac.Categories.FirstOrDefault(c => c.Name == "Фурнитура");
            if (cat != null)
            {
                return RedirectToAction("Products", "Categories", new { id = cat.Id });
            }
            return HttpNotFound();
        }
	}
}