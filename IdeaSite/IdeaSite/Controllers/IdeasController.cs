using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdeaSite.Models;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace IdeaSite.Controllers
{
    public class IdeasController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Ideas
        public ActionResult Index()
        {
            return View(db.Ideas.ToList());
        }

        // GET: Ideas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }

            /* BEFORE RUNNING THIS, GO TO WEB.CONFIG (THE SECOND ONE).
             * THERE WILL BE A LIST OF KEY/VALUE PAIRS IN APPSETTINGS.
             * CHANGE THE VALUE OF KEY "serverPath" TO SOMEWHERE ON 
             * YOUR MACHINE*/
            var appSettings = ConfigurationManager.AppSettings;

            // store path to server location of the file storage
            var connectionInfo = appSettings["serverPath"];

            // combine the server location and the name of the new folder to be created
            var ideaFolder = string.Format("{0}{1}", connectionInfo, id);
            DirectoryInfo dir = new DirectoryInfo(ideaFolder);

            // Store the files from the desired file folder
            var files = dir.GetFiles();

            ViewBag.path = ideaFolder;
            ViewBag.files = files;

            return View(idea);
        }

        // GET: Ideas/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,description,category,creationDate,statusCode,denialReason")] Idea idea, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                idea.creationDate = DateTime.Now;
                db.Ideas.Add(idea);
                db.SaveChanges();

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the file storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var storagePath = string.Format(@"{0}{1}", connectionInfo, idea.ID);

                DirectoryInfo di = Directory.CreateDirectory(storagePath);

                try
                {
                    // loop through the uploads and pull out each file from it.
                    for (int i = 0; i < files.Count(); ++i)
                    {
                        if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                        {
                            // store the name of the file
                            var name = Path.GetFileName(files.ElementAt(i).FileName);

                            // create new object to reference the loaction of the new file and the ID of the idea to which it belongs.
                            var file = new Models.File
                            {
                                fileName = string.Format("{0}\\{1}", storagePath, name),
                                ID = idea.ID
                            };

                            files.ElementAt(i).SaveAs(string.Format("{0}\\{1}", storagePath, name));
                        }
                    }
                }

                catch
                {
                    Debug.WriteLine("Upload failed");
                    ViewBag.Message = "Upload failed";
                    return RedirectToAction("Create");
                }

                return RedirectToAction("Index");
            }

            return View(idea);
        }

        // GET: Ideas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,description,category,creationDate,statusCode,denialReason")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                idea.creationDate = DateTime.Now;
                db.Entry(idea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(idea);
        }


        // must delete all comments as well
        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Idea idea = db.Ideas.Find(id);
            db.Comments.RemoveRange(db.Comments.Where(com => com.ideaID == id));

            var appSettings = ConfigurationManager.AppSettings;

            // store path to server location of the file storage
            var connectionInfo = appSettings["serverPath"];
            
            // combine the server location and the name of the new folder to be created
            var storagePath = string.Format(@"{0}{1}", connectionInfo, idea.ID);
            Directory.Delete(storagePath, true);

            db.Ideas.Remove(idea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult Download(string filepath, string fileName)
        {
            return File(filepath, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
