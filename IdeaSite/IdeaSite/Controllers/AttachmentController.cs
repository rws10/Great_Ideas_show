﻿using IdeaSite.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace IdeaSite.Controllers
{
    public class AttachmentController : Controller
    {
        private Great_Ideas db = new Great_Ideas();

        // GET: Attachment
        [ChildActionOnly]
        public ActionResult Display(int ideaID)
        {
            List<Attachment> attachments = new List<Models.Attachment>();
            attachments = db.Attachments.Where(attachment => attachment.ideaID == ideaID).ToList();

            ViewBag.attachments = attachments;

            return PartialView();
        }

        public FileResult Download(string attachmentpath, string attachmentName)
        {
            return File(attachmentpath, System.Net.Mime.MediaTypeNames.Application.Octet, attachmentName);
        }

    }

}