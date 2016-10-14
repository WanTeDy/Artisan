using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Artisan.Controllers;
using System.Diagnostics;

namespace Artisan
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            if (objErr is HttpException)
            {
                var httpEx = objErr as HttpException;
                if (httpEx.GetHttpCode() == 404)
                {
                    Response.Redirect("/Error/NotFound");
                }
                else
                {
                    string err = " HERE IS ERROR: " +
                    "Error for: " + Request.Url.ToString() +
                    "\nError message: " + objErr.Message.ToString() +
                    "\nStackTrace: " + objErr.StackTrace.ToString().Substring(0,100) +
                    "\n";
                    logger.Error(err);
                    Server.ClearError();
                    Response.Redirect("/Error/InternalServerError");
                }
            }
            else
            {
                string err = " HERE IS ERROR: " +
                    "Error for: " + Request.Url.ToString() +
                    "\nError message: " + objErr.Message.ToString() +
                    "\nStackTrace: " + objErr.StackTrace.ToString().Substring(0, 100) +
                    "\n";
                logger.Error(err);
                Server.ClearError();
                Response.Redirect("/Error/InternalServerError");
            }            
        }
    }
}
