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
            List<Attachment> attachments = new List<Models.Attachment>();
            attachments = db.Attachments.Where(attachment => attachment.ownIdea.ID == ideaID).ToList();

            ViewBag.attachments = attachments;

            return PartialView();
        }

        /* public ActionResult _Index(int? id)
         {
             var model = new FileSelectionViewModel();

             model.idea = db.Ideas.Find(id);

             foreach (var attachment in db.Attachments)
             {
                 var editorViewModel = new FileSelectorEditorViewModel()
                 {
                     ID = attachment.ID,
                     Name = string.Format("{0}\\{1}", attachment.storageLocation, attachment.name),
                     Selected = true
                 };
                 model.Attachs.Add(editorViewModel);
             }
             return View(model);
         }


         [HttpPost]
         public ActionResult SubmitSelected(FileSelectionViewModel model)
         {
             // get the ids of the items selected:
             var selectedIds = model.getSelectedIds();

             // Use the ids to retrieve the records for the selected people
             // from the database:
             var selectedAttachments = from x in db.Attachments
                                  where selectedIds.Contains(x.ID)
                                  select x;

             // Process according to your requirements:
             foreach (var attachment in selectedAttachments)
             {
                 // in here is where I will delete the attachments based on what was selected.
                 System.Diagnostics.Debug.WriteLine(
                     string.Format("{0}\\{1}", attachment.storageLocation, attachment.name));
             }

             // Redirect somewhere meaningful (probably to somewhere showing 
             // the results of your processing):
             return RedirectToAction("Index");
         }*/

        public FileResult Download(string attachmentpath, string attachmentName)
        {
            return File(attachmentpath, System.Net.Mime.MediaTypeNames.Application.Octet, attachmentName);
        }

    }

}