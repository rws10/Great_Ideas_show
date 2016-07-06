using IdeaSite.Models;
using log4net;
using System;
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

            StringBuilder sb = new StringBuilder();
            sb.Append(ctx.Request.Url.ToString() + System.Environment.NewLine);
            sb.Append("Source:" + System.Environment.NewLine + ctx.Server.GetLastError().Source.ToString());
            sb.Append("Message:" + System.Environment.NewLine + ctx.Server.GetLastError().Message.ToString());
            sb.Append("Stack Trace:" + System.Environment.NewLine + ctx.Server.GetLastError().StackTrace.ToString());

            log.Error("An unhandled error occured.", ctx.Error);

            //REDIRECT USER TO ERROR PAGE
            Response.Redirect("~/Views/Shared/Error.cshtml");
        }
    }
}
