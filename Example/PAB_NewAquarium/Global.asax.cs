using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PAB_NewAquarium.Helpers.DataModel;

namespace PAB_NewAquarium
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            // Check if the error is an HTTP 404 Not Found
            if (exception is HttpException httpException && httpException.GetHttpCode() == 404)
            {
                // Handle the 404 Not Found error
                Response.Clear();
                Server.ClearError();

                // Redirect to the ErrorNotFound action in HomeController
                Response.RedirectToRoute("Default", new { controller = "Home", action = "NotFoundPage" });
            }
        }
    }
}