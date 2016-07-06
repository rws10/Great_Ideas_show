using IdeaSite.Models;
using log4net;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IdeaSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogHelper.GetLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // place the code to delete an idea marked with a status of "Delete" here
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;

            log.Error("An unhandled error occured.", ctx.Error);

            Response.RedirectToRoute("Error", new { controller = "Error", action = "Index"});
            this.Context.ClearError();
        }
    }
}
