using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Artisan.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.IO;
using System.Data.Entity;

namespace Artisan.Controllers
{
    [Authorize]
    public class WebmasterController : Controller
    {
        private Cloudinary _cloudinary = new Cloudinary(new Account(
                Properties.Settings.Default.CloudName,
                Properties.Settings.Default.ApiKey,
                Properties.Settings.Default.ApiSecret));
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public WebmasterController()
        {
        }

        public WebmasterController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Webmaster/Index
        public ActionResult Index()
        {
            ViewBag.Title = "Artisan Webmaster";
            return RedirectToAction("Categories");
        }

        //
        // GET: /Webmaster/Categories
        public ActionResult Categories(int? id)
        {
            ArtisanContext ac = new ArtisanContext();
            ViewBag.cloud = _cloudinary;
            ViewBag.Title = "Artisan Webmaster категории";
            if (id == null)
            {
                return View(ac.Categories.Include("Image"));
            }
            Category category = ac.Categories.Include("Image").FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                return View("ShowCategory", category);
            }
            return HttpNotFound();

        }

        // GET: /Webmaster/AddCategory
        public ActionResult AddCategory()
        {
            ViewBag.Title = "Artisan Webmaster добавление категории";
            return View();
        }
        // POST: /Webmaster/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(Category category, string imgDescription, HttpPostedFileBase upload)
        {
            ViewBag.Title = "Artisan Webmaster добавление категории";
            if (!ModelState.IsValid)
            {
                return View();
            }
            ArtisanContext ac = new ArtisanContext();
            if (ac.Categories.FirstOrDefault(item => item.Name == category.Name) != null)
            {
                ModelState.AddModelError("Name", "Такая категория уже существует");
                return View();
            }
            int imageId = 0;
            if (upload != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(upload.FileName, upload.InputStream)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image img = new Image
                    {
                        Description = imgDescription,
                        PublicId = uploadResult.PublicId
                    };
                    ac.Images.Add(img);
                    ac.SaveChanges();
                    imageId = img.Id;
                }
            }

            category.ImageId = imageId;
            ac.Categories.Add(category);
            ac.SaveChanges();
            return RedirectToAction("Categories");
        }

        // GET: /Webmaster/EditCategory
        public ActionResult EditCategory(int? id)
        {
            ViewBag.Title = "Artisan Webmaster редактирование категории";
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Category category = ac.Categories.Include("Image").FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                ViewBag.cloud = _cloudinary;
                return View(category);
            }
            return HttpNotFound();
        }
        // POST: /Webmaster/EditCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category category, string imgDescription, HttpPostedFileBase upload)
        {
            ViewBag.Title = "Artisan Webmaster редактирование категории";
            ViewBag.cloud = _cloudinary;
            ArtisanContext ac = new ArtisanContext();
            if (category != null)
            {
                category.Image = ac.Images.FirstOrDefault(c => c.Id == category.ImageId);
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            var editItem = ac.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (!editItem.Name.Equals(category.Name) && ac.Categories.FirstOrDefault(item => item.Name == category.Name) != null)
            {
                ModelState.AddModelError("Name", "Такая категория уже существует");
                return View(category);
            }
            if (imgDescription != null)
            {
                if (imgDescription.Length > 50)
                {
                    ViewBag.Error = "Описание должно быть не длиннее 50 символов";
                    return View(category);
                }
            }
            if (category.ImageId != 0)
            {
                category.Image.Description = imgDescription;
            }
            if (upload != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(upload.FileName, upload.InputStream)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (category.ImageId != 0)
                    {
                        _cloudinary.DeleteResources(category.Image.PublicId);
                    }
                    category.Image.PublicId = uploadResult.PublicId;
                }
            }
            ac.Entry(category.Image).State = EntityState.Modified;
            editItem.Name = category.Name;
            editItem.Description = category.Description;
            ac.Entry(editItem).State = EntityState.Modified;
            ac.SaveChanges();
            return RedirectToAction("Categories");
        }

        // GET: /Webmaster/DeleteCategory        
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Category category = ac.Categories.Include("Image").FirstOrDefault(c => c.Id == id);
            if (category == null)
                HttpNotFound();
            Image img = ac.Images.FirstOrDefault(i => i.Id == category.ImageId);
            if (category.ImageId != 0)
            {
                category.Image.Categories.Clear();
                _cloudinary.DeleteResources(category.Image.PublicId);
                ac.Entry(category).State = EntityState.Deleted;
                ac.SaveChanges();
                ac.Entry(img).State = EntityState.Deleted;
            }
            else
            {
                ac.Entry(category).State = EntityState.Deleted;
            }
            ac.SaveChanges();
            return RedirectToAction("Categories");
        }

        //
        // GET: /Webmaster/Products
        public ActionResult Products(int? id)
        {
            ViewBag.Title = "Artisan Webmaster продукты";
            ArtisanContext ac = new ArtisanContext();
            ViewBag.cloud = _cloudinary;
            if (id == null)
            {
                return View(ac.Products.Include("Image").Include("Category"));
            }
            Product product = ac.Products.Include("Image").Include("Category").FirstOrDefault(c => c.Id == id);
            if (product != null)
            {
                return View("ShowProduct", product);
            }
            return HttpNotFound();
        }

        // GET: /Webmaster/AddProduct
        public ActionResult AddProduct()
        {
            ViewBag.Title = "Artisan Webmaster добавление продукта";
            ArtisanContext ac = new ArtisanContext();
            ViewBag.Categories = new SelectList(ac.Categories, "Id", "Name");
            return View();
        }
        // POST: /Webmaster/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product product, string imgDescription, HttpPostedFileBase upload)
        {
            ViewBag.Title = "Artisan Webmaster добавление продукта";
            ArtisanContext ac = new ArtisanContext();
            ViewBag.Description = imgDescription;
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(ac.Categories, "Id", "Name");
                return View(product);
            }
            if (ac.Products.FirstOrDefault(item => item.Name == product.Name) != null)
            {
                ModelState.AddModelError("Name", "Такой продукт уже существует");
                return View(product);
            }
            if (imgDescription != null)
            {
                if (imgDescription.Length > 50)
                {
                    ViewBag.Error = "Описание должно быть не длиннее 50 символов";
                    return View(product);
                }
            }
            int imageId = 0;
            if (upload != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(upload.FileName, upload.InputStream)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image img = new Image
                    {
                        Description = imgDescription,
                        PublicId = uploadResult.PublicId
                    };
                    ac.Images.Add(img);
                    ac.SaveChanges();
                    imageId = img.Id;
                }
            }
            product.ImageId = imageId;
            ac.Products.Add(product);
            ac.SaveChanges();
            return RedirectToAction("Products");
        }

        // GET: /Webmaster/EditProduct
        public ActionResult EditProduct(int? id)
        {
            ViewBag.Title = "Artisan Webmaster редактирование категории";
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Product product = ac.Products.Include("Image").Include("Category").FirstOrDefault(pr => pr.Id == id);
            if (product != null)
            {
                ViewBag.Categories = new SelectList(ac.Categories, "Id", "Name");
                ViewBag.cloud = _cloudinary;
                return View(product);
            }
            return HttpNotFound();
        }
        // POST: /Webmaster/EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product, string imgDescription, HttpPostedFileBase upload)
        {
            ViewBag.Title = "Artisan Webmaster редактирование категории";
            ViewBag.cloud = _cloudinary;
            ArtisanContext ac = new ArtisanContext();
            if (product != null)
            {
                product.Image = ac.Images.FirstOrDefault(pr => pr.Id == product.ImageId);
            }
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var editItem = ac.Products.FirstOrDefault(pr => pr.Id == product.Id);
            if (!editItem.Name.Equals(product.Name) && ac.Products.FirstOrDefault(item => item.Name == product.Name) != null)
            {
                ModelState.AddModelError("Name", "Такой продукт уже существует");
                return View(product);
            }
            if (imgDescription != null)
            {
                if (imgDescription.Length > 50)
                {
                    ViewBag.Error = "Описание должно быть не длиннее 50 символов";
                    return View(product);
                }
            }
            if (product.ImageId != 0)
            {
                product.Image.Description = imgDescription;
            }
            if (upload != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(upload.FileName, upload.InputStream)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (product.ImageId != 0)
                    {
                        _cloudinary.DeleteResources(product.Image.PublicId);
                    }
                    product.Image.PublicId = uploadResult.PublicId;
                }
            }
            ac.Entry(product.Image).State = EntityState.Modified;
            editItem.Name = product.Name;
            editItem.Price = product.Price;
            editItem.Description = product.Description;
            editItem.CategoryId = product.CategoryId;
            ac.Entry(editItem).State = EntityState.Modified;
            ac.SaveChanges();
            return RedirectToAction("Products");
        }

        // GET: /Webmaster/DeleteProduct   
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Product product = ac.Products.Include("Image").Include("Category").FirstOrDefault(pr => pr.Id == id);
            if (product == null)
                HttpNotFound();
            Image img = ac.Images.FirstOrDefault(i => i.Id == product.ImageId);
            if (product.ImageId != 0)
            {
                product.Image.Products.Clear();
                _cloudinary.DeleteResources(product.Image.PublicId);
                ac.Entry(product).State = EntityState.Deleted;
                ac.SaveChanges();
                ac.Entry(img).State = EntityState.Deleted;
            }
            else
            {
                ac.Entry(product).State = EntityState.Deleted;
            }
            ac.SaveChanges();
            return RedirectToAction("Products");
        }
        //
        // GET: /Webmaster/Gallery
        public ActionResult Gallery()
        {
            ViewBag.Title = "Artisan Webmaster галлерея";
            ArtisanContext ac = new ArtisanContext();
            ViewBag.cloud = _cloudinary;
            return View(ac.Images.Where(i => i.Gallery));
        }

        // GET: /Webmaster/AddGallery
        public ActionResult AddGallery()
        {
            ViewBag.Title = "Artisan Webmaster добавление фото";
            return View();
        }
        // POST: /Webmaster/AddGallery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGallery(string description, HttpPostedFileBase[] masUpload)
        {
            ViewBag.Title = "Artisan Webmaster добавление фото";
            if (masUpload != null)
            {
                if (description != null)
                {
                    if (description.Length > 50)
                    {
                        ViewBag.Error = "Описание должно быть не длиннее 50 символов";
                        return View(description);
                    }
                }
                ArtisanContext ac = new ArtisanContext();
                foreach (var upload in masUpload)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(upload.FileName, upload.InputStream)
                    };
                    var uploadResult = _cloudinary.Upload(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Image img = new Image
                        {
                            Description = description,
                            PublicId = uploadResult.PublicId,
                            Gallery = true
                        };
                        ac.Images.Add(img);
                        ac.SaveChanges();
                    }
                }
                return RedirectToAction("Gallery");
            }
            ViewBag.description = description;
            ViewBag.upload = "Вы не выбрали фото!";
            return View();
        }

        // Get: /Webmaster/EditGallery
        public ActionResult EditGallery(int? id)
        {
            ViewBag.Title = "Artisan Webmaster редактирование фотографии";
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Image image = ac.Images.FirstOrDefault(img => img.Id == id);
            if (image != null)
            {
                ViewBag.cloud = _cloudinary;
                return View(image);
            }
            return HttpNotFound();
        }

        // POST: /Webmaster/EditGallery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGallery(Image image, HttpPostedFileBase upload)
        {
            ViewBag.Title = "Artisan Webmaster редактирование фотографии";
            ViewBag.cloud = _cloudinary;
            ArtisanContext ac = new ArtisanContext();
            Image img = ac.Images.FirstOrDefault(i => i.Id == image.Id);
            if (img == null)
            {
                return HttpNotFound();
            }
            if (image.Description != null && image.Description.Length > 50)
            {
                ViewBag.Error = "Описание должно быть не длиннее 50 символов";
                image.PublicId = img.PublicId;
                return View(image);
            }
            if (image.Id == 0)
            {
                image.Description = "";
            }
            if (upload != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(upload.FileName, upload.InputStream)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (image.Id != 0)
                    {
                        _cloudinary.DeleteResources(image.PublicId);
                    }
                    img.PublicId = uploadResult.PublicId;
                }
            }
            img.Description = image.Description;
            ac.Entry(img).State = EntityState.Modified;
            ac.SaveChanges();
            return RedirectToAction("Gallery");
        }

        // GET: /Webmaster/DeleteGallery  
        public ActionResult DeleteGallery(int? id)
        {
            if (id == null)
                HttpNotFound();
            ArtisanContext ac = new ArtisanContext();
            Image img = ac.Images.FirstOrDefault(i => i.Id == id);
            if (img == null || !img.Gallery)
                HttpNotFound();
            _cloudinary.DeleteResources(img.PublicId);
            ac.Entry(img).State = EntityState.Deleted;
            ac.SaveChanges();
            return RedirectToAction("Gallery");
        }
        //
        // GET: /Webmaster/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Title = "Artisan вход";

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Webmaster/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.Title = "Artisan вход";
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Вы ввели неправильное имя или пароль");
                    return View(model);
            }
        }

        //
        // GET: /Webmaster/VerifyCode
        /*[AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }*/

        //
        // POST: /Webmaster/VerifyCode
        /*[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Вы ввели неправильно код подтверждения");
                    return View(model);
            }
        }*/

        //
        // GET: /Webmaster/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Title = "Artisan регистрация";
            return View();
        }

        //
        // POST: /Webmaster/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Title = "Artisan регистрация";
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Webmaster/ConfirmEmail
        /* [AllowAnonymous]
         public async Task<ActionResult> ConfirmEmail(string userId, string code)
         {
             if (userId == null || code == null)
             {
                 return View("Error");
             }
             var result = await UserManager.ConfirmEmailAsync(userId, code);
             return View(result.Succeeded ? "ConfirmEmail" : "Error");
         }*/

        //
        // GET: /Webmaster/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Title = "Artisan восстановление пароля";
            return View();
        }

        //
        // POST: /Webmaster/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.Title = "Artisan восстановление пароля";
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || (await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Webmaster", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Webmaster");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Webmaster/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Title = "Artisan подтверждение отправки";
            return View();
        }

        //
        // GET: /Webmaster/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
                HttpNotFound();
            ViewBag.Title = "Artisan сброс пароля";
            return View();
        }

        //
        // POST: /Webmaster/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ViewBag.Title = "Artisan сброс пароля";
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ViewBag.Message = "Такого пользователя не существует";
                return View("ResetPasswordConfirmation", false);
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Message = "Пароль успешно изменён";
                return View("ResetPasswordConfirmation", true);
            }
            AddErrors(result);
            return View();
        }

        //
        // POST: /Webmaster/ExternalLogin
        /* [HttpPost]
         [AllowAnonymous]
         [ValidateAntiForgeryToken]
         public ActionResult ExternalLogin(string provider, string returnUrl)
         {
             // Request a redirect to the external login provider
             return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
         }*/

        //
        // GET: /Webmaster/SendCode
        /* [AllowAnonymous]
         public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
         {
             var userId = await SignInManager.GetVerifiedUserIdAsync();
             if (userId == null)
             {
                 return View("Error");
             }
             var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
             var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
             return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
         }*/

        //
        // POST: /Webmaster/SendCode
        /* [HttpPost]
         [AllowAnonymous]
         [ValidateAntiForgeryToken]
         public async Task<ActionResult> SendCode(SendCodeViewModel model)
         {
             if (!ModelState.IsValid)
             {
                 return View();
             }

             // Generate the token and send it
             if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
             {
                 return View("Error");
             }
             return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
         }*/

        //
        // GET: /Webmaster/ExternalLoginCallback
        /* [AllowAnonymous]
         public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
         {
             var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
             if (loginInfo == null)
             {
                 return RedirectToAction("Login");
             }

             // Sign in the user with this external login provider if the user already has a login
             var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
             switch (result)
             {
                 case SignInStatus.Success:
                     return RedirectToLocal(returnUrl);
                 case SignInStatus.LockedOut:
                     return View("Lockout");
                 case SignInStatus.RequiresVerification:
                     return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                 case SignInStatus.Failure:
                 default:
                     // If the user does not have an account, then prompt the user to create an account
                     ViewBag.ReturnUrl = returnUrl;
                     ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                     return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
             }
         }*/

        //
        // POST: /Webmaster/ExternalLoginConfirmation
        /*     [HttpPost]
             [AllowAnonymous]
             [ValidateAntiForgeryToken]
             public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
             {
                 if (User.Identity.IsAuthenticated)
                 {
                     return RedirectToAction("Index", "Manage");
                 }

                 if (ModelState.IsValid)
                 {
                     // Get the information about the user from the external login provider
                     var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                     if (info == null)
                     {
                         return View("ExternalLoginFailure");
                     }
                     var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                     var result = await UserManager.CreateAsync(user);
                     if (result.Succeeded)
                     {
                         result = await UserManager.AddLoginAsync(user.Id, info.Login);
                         if (result.Succeeded)
                         {
                             await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                             return RedirectToLocal(returnUrl);
                         }
                     }
                     AddErrors(result);
                 }

                 ViewBag.ReturnUrl = returnUrl;
                 return View(model);
             }*/

        //
        // POST: /Webmaster/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Webmaster/ExternalLoginFailure
        /*  [AllowAnonymous]
          public ActionResult ExternalLoginFailure()
          {
              return View();
          }
          */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}