using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdeaSite.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Index()
        {
            return View("Error");
        }
        
        public ViewResult Forbidden()
        {
            Response.StatusCode = 403;
            return View("Forbidden");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ViewResult InternalServerError()
        {
            Response.StatusCode = 404;
            return View("InternalServerError");
        }

        public ViewResult Unauthorized()
        {
            Response.StatusCode = 401;
            return View("Unauthorized");
        }


    }
}