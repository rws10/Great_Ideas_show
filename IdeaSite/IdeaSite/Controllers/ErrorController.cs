using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace IdeaSite.Controllers
{
    public class ErrorController : Controller
    {

        String filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";

        public ViewResult Index()
        {
            //var sw = new System.IO.StreamWriter(filename, true);
            using (StreamWriter tw = new StreamWriter(@"C:\Users\alex\Desktop\log.txt"))
            {
                tw.Write("UNSPECIFIED ERROR");
            }
            /*
            sw.WriteLine(DateTime.Now.ToString() + " UNKNOWN ERROR");
            sw.Close();
            System.IO.File.WriteAllText(@"C:\Users\alex\Desktop\log.txt", "text");
            */
            return View("Error");
        }
        
        public ViewResult Forbidden()
        {
            /*
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(DateTime.Now.ToString() + " STATUSCODE 403");
            sw.Close();
            System.IO.File.WriteAllText(@"C:\Users\alex\Desktop\log.txt", "text");
            */
            using (StreamWriter tw = new StreamWriter(@"C:\Users\alex\Desktop\log.txt"))
            {
                tw.Write("ERROR CODE 403: FORBIDDEN");
            }
            Response.StatusCode = 403;
            return View("Forbidden");
        }

        public ViewResult NotFound()
        {
            /*
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(DateTime.Now.ToString() + " STATUSCODE 404");
            sw.Close();
            System.IO.File.WriteAllText(@"C:\Users\alex\Desktop\log.txt", "text");
            */
            using (StreamWriter tw = new StreamWriter(@"C:\Users\alex\Desktop\log.txt"))
            {
                tw.Write("ERROR CODE 404: NOT FOUND");
            }
            Response.StatusCode = 404;
            return View("Error/NotFound");
        }

        public ViewResult InternalServerError()
        {
            /*
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(DateTime.Now.ToString() + " STATUSCODE 500");
            sw.Close();
            System.IO.File.WriteAllText(@"C:\Users\alex\Desktop\log.txt", "text");
            */
            using (StreamWriter tw = new StreamWriter(@"C:\Users\alex\Desktop\log.txt"))
            {
                tw.Write("ERROR CODE 500: INTERNAL SERVER ERROR");
            }
            Response.StatusCode = 500;
            return View("InternalServerError");
        }

        public ViewResult Unauthorized()
        {
            /*
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(DateTime.Now.ToString() + " STATUSCODE 401");
            sw.Close();
            System.IO.File.WriteAllText(@"C:\Users\alex\Desktop\log.txt", "text");
            */
            using (StreamWriter tw = new StreamWriter(@"C:\Users\alex\Desktop\log.txt"))
            {
                tw.Write("ERROR CODE 401: UNAUTHORIZED");
            }
            Response.StatusCode = 401;
            return View("Unauthorized");
        }


    }
}