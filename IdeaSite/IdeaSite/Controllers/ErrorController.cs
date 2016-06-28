using System.Web.Mvc;
using System.IO;

namespace IdeaSite.Controllers
{
    public class ErrorController : Controller
    {

        public ViewResult Index()
        {
            using (StreamWriter tw = new StreamWriter(@"C:\Users\simmonr1\Desktop\log.txt"))
            {
                tw.Write("UNSPECIFIED ERROR");
            }
            return View("Error");
        }
        
        public ViewResult Forbidden()
        {
            using (StreamWriter tw = new StreamWriter(@"C:\Users\simmonr1\Desktop\log.txt"))
            {
                tw.WriteLine("ERROR CODE 403: FORBIDDEN");
            }
            Response.StatusCode = 403;
            return View("Forbidden");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            using (StreamWriter tw = new StreamWriter(@"C:\Users\simmonr1\Desktop\log.txt"))
            {
                tw.WriteLine("ERROR CODE 404: NOT FOUND");
            }
            return View("NotFound");
        }

        //public ViewResult InternalServerError()
        //{
        //    Response.StatusCode = 500;
        //    using (StreamWriter tw = new StreamWriter(@"C:\Users\simmonr1\Desktop\log.txt"))
        //    {
        //        tw.WriteLine("ERROR CODE 500: INTERNAL SERVER ERROR");
        //    }
        //    return View("InternalServerError");
        //}

        public ViewResult Unauthorized()
        {
            Response.StatusCode = 401;
            using (StreamWriter tw = new StreamWriter(@"C:\Users\simmonr1\Desktop\log.txt"))
            {
                tw.WriteLine("ERROR CODE 401: UNAUTHORIZED");
            }
            return View("Unauthorized");
        }
    }
}