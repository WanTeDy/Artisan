using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;

namespace Artisan.Controllers
{
    public class ErrorController : Controller
    {
        private HttpStatusCode _httpStatusCode;
        private string _statusCodeDescription;

        // 500
        public ActionResult InternalServerError()
        {
            ViewBag.Title = "Artisan общая ошибка";
            SetResponse(HttpStatusCode.InternalServerError);
            // message to view from TempData
            
            return View();
        }

        // 404
        public ViewResult NotFound()
        {
            ViewBag.Title = "Artisan 404 страница не найдена";
            SetResponse(HttpStatusCode.NotFound);
            return View();
        }        

        private void SetResponse(HttpStatusCode httpStatusCode)
        {
            _httpStatusCode = httpStatusCode;
            _statusCodeDescription = HttpWorkerRequest.GetStatusDescription((int)_httpStatusCode);
            Response.StatusCode = (int)_httpStatusCode;
            Response.StatusDescription = _statusCodeDescription;
        }

	}
}