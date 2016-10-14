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
    public class CategoriesController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));


        public ActionResult Products(int? id)
        {
            if (id == null)
                return HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Category category = ac.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                ViewBag.Title = "Artisan " + category.Name;
                ViewBag.cloud = _cloudinary;
                ViewBag.Category = category;
                return View(ac.Products.Include("Image").Include("Category").Where(pr => pr.CategoryId == category.Id));
            }
            return HttpNotFound();
        }        

    }
}