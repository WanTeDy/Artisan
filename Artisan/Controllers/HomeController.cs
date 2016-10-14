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
//[HandleError]
    public class HomeController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        public ActionResult Index()
        {
            ViewBag.Title = "Artisan главная";
            ArtisanContext ac = new ArtisanContext();
            ViewBag.cloud = _cloudinary;
            return View(ac.Categories.Include("Image"));
        }        
    }
}