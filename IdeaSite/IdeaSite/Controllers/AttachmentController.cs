using IdeaSite.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace IdeaSite.Controllers
{
    public class AttachmentController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Attachment
        [ChildActionOnly]
        public ActionResult Display(int ideaID)
        {
            IEnumerable<Models.Attachment> attachments = new List<Models.Attachment>();
            attachments = db.Attachments.Where(attachment => attachment.ideaID == ideaID).ToList();

            ViewBag.attachments = attachments;

            return PartialView();
        }
    }
}