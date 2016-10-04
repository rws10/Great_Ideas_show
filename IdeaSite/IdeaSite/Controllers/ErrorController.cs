using System.Web.Mvc;
using System.IO;

namespace IdeaSite.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult AnError()
        {
            return View("Error");
        }
    }
}