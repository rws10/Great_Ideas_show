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
using System.Net.Mail;

namespace IdeaSite.Controllers
{
    public class IdeasController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        internal static void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            MailMessage msg = new MailMessage();
            msg.From = fromAddress;
            msg.To.Add(toAddress);
            msg.Body = body;
            msg.IsBodyHtml = true;
            msg.Subject = subject;
            SmtpClient smt = new SmtpClient("smtp-mail.outlook.com ");
            smt.Port = 587;
            smt.Credentials = new NetworkCredential("teamzed@outlook.com", "Boobies69");
            smt.EnableSsl = true;
            smt.Send(msg);
        }

        // GET: Ideas
        public ActionResult Index(string searchBy, string search, string sortByStatus)
        {

            IEnumerable<Idea> results = new List<Idea>();
            //string[] sep = new string[] { (" ") };
            string[] searchTerms;
            //search.Remove(' ');

            if (search != null)
            {
                searchTerms = search.Trim().Split(' ');
                //for (int i = 0; i < searchTerms.Length; ++i) { searchTerms[i] = searchTerms[i].Trim(); }
            }
            else { searchTerms = null; }

            var ideas = db.Ideas.ToList();
            List<int[]> matches = new List<int[]>();
            foreach (Idea idea in ideas)
            {
                int[] match = new int[2];
                match[0] = idea.ID;
                match[1] = 0;
                matches.Add(match);
            }

            if (searchBy == "Title" && search != null)
            {
                for (int i = 0; i < searchTerms.Length; ++i)
                {
                    var term = searchTerms[i];
                    results = results.Concat(db.Ideas.Where(x => x.title.Contains(term)).ToList());
                }
            }
            else if (searchBy == "Description" && search != null)
            {
                for (int i = 0; i < searchTerms.Length; ++i)
                {
                    var term = searchTerms[i];
                    results = results.Concat(db.Ideas.Where(x => x.body.Contains(term)).ToList());
                }
            }
            else if (searchBy == "All" && search != null)
            {
                for (int i = 0; i < searchTerms.Length; ++i)
                {
                    var term = searchTerms[i];
                    results = results.Concat(db.Ideas.Where(x => x.title.Contains(term)).ToList());
                    results = results.Concat(db.Ideas.Where(x => x.body.Contains(term)).ToList());
                }
            }

            if (sortByStatus == "All") { /* do nothing */ }
            else { results = results.Where(x => x.statusCode == sortByStatus); }

            if (search != null)
            {
                foreach (Idea idea in results)
                {
                    foreach (var match in matches)
                    {
                        if (idea.ID == match[0]) { ++match[1]; }
                    }
                }

                // I think these are the same
                matches.OrderBy(x => x[1]);
                matches = matches.OrderBy(x => x[1]).ToList();
                matches.Reverse();

                results.Distinct();
                IEnumerable<Idea> finalResults = new List<Idea>();

                for (int i = 0; i < matches.Count(); ++i)
                {
                    foreach (var idea in results)
                    {
                        if (idea.ID == matches[i][0])
                        {
                            finalResults = finalResults.Concat(db.Ideas.Where(x => x.ID == idea.ID).ToList());
                        }

                    }
                }
                finalResults = finalResults.Distinct();
                return View(finalResults);
            }
            else
            {
                IEnumerable<Idea> origResults = new List<Idea>();
                origResults = db.Ideas.ToList();
                origResults = origResults.Reverse();
                return View(origResults);
            }
        }


        // GET: Ideas/Details/
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
            var ideaFolder = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);
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
        public ActionResult Create([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason")] Idea idea, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                idea.cre_user = "Administrator";
                idea.cre_date = DateTime.Now;
                db.Ideas.Add(idea);
                db.SaveChanges();

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the file storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);

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
                                storageLocation = string.Format("{0}\\{1}", storagePath, name),
                                cre_date = DateTime.Now,
                                ID = idea.ID
                            };

                            files.ElementAt(i).SaveAs(string.Format("{0}\\{1}", storagePath, name));

                            db.Files.Add(file);
                            db.SaveChanges();
                        }
                    }
                }

                catch
                {
                    Debug.WriteLine("Upload failed");
                    ViewBag.Message = "Upload failed";
                    return RedirectToAction("Create");
                }

                string subject = string.Format("New Idea Submission: {0}", idea.title);

                string body = string.Format("{0} has submitted an Idea on Great Ideas:" +
                    "<br/><br/>{1}:" +
                    "<br/>{2}" +
                    "<br/><br/>Please go to <a href=\"http://localhost:52398/Ideas/Approval/{2}\">Great Ideas</a> to submit approval.",
                    idea.cre_user, idea.title, idea.body);

                MailAddress from = new MailAddress("teamzed@outlook.com");
                MailAddress to = new MailAddress("rws10@live.com");


                //SendEmail(from, to, subject, body);

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

            idea.statusCodes = new[]
            {
                new SelectListItem { Value = "Approved", Text = "Approved" },
                new SelectListItem { Value = "Archived", Text = "Archived" },
                new SelectListItem { Value = "Project Submission", Text = "Project Submission" },
            };

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
        public ActionResult Edit([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason")] Idea idea, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(idea).State = EntityState.Modified;
                var currentIdea = db.Ideas.FirstOrDefault(p => p.ID == idea.ID);
                if (currentIdea == null)
                {
                    return HttpNotFound();
                }

                currentIdea.title = idea.title;
                currentIdea.body = idea.body;
                currentIdea.statusCode = idea.statusCode;
                currentIdea.statusCodes = idea.statusCodes;
                currentIdea.denialReason = idea.denialReason;

                db.SaveChanges();

                string subject = string.Format("An idea has been edited: {0}", idea.title);

                string body = string.Format("{0} has Edited an Idea on Great Ideas:" +
                    "<br/><br/>{1}:" +
                    "<br/>{2}" +
                    "<br/><br/>Please go to <a href=\"http://localhost:52398/Ideas/Approval/{3}\">Great Ideas</a> to submit approval.",
                    idea.cre_user, idea.title, idea.body, idea.ID);

                MailAddress from = new MailAddress("teamzed@outlook.com");
                MailAddress to = new MailAddress("rws10@live.com");


                //SendEmail(from, to, subject, body);

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the file storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);

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
                                storageLocation = string.Format("{0}\\{1}", storagePath, name),
                                cre_date = DateTime.Now,
                                ID = idea.ID
                            };

                            files.ElementAt(i).SaveAs(string.Format("{0}\\{1}", storagePath, name));

                            db.Files.Add(file);
                            db.SaveChanges();
                        }
                    }
                }

                catch
                {
                    Debug.WriteLine("Upload failed");
                    ViewBag.Message = "Upload failed";
                    return RedirectToAction("Edit");
                }

                return RedirectToAction("Index");
            }
            //ViewBag.files = files;
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
            db.Files.RemoveRange(db.Files.Where(fil => fil.ideaID == id));
            db.Comments.RemoveRange(db.Comments.Where(com => com.ideaID == id));

            var appSettings = ConfigurationManager.AppSettings;

            // store path to server location of the file storage
            var connectionInfo = appSettings["serverPath"];

            // combine the server location and the name of the new folder to be created
            var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);

            if (Directory.Exists(storagePath))
            {
                Directory.Delete(storagePath, true);
            }

            db.Ideas.Remove(idea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Ideas/Approve/5
        public ActionResult Approval(int? id)
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
            var ideaFolder = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);
            DirectoryInfo dir = new DirectoryInfo(ideaFolder);

            // Store the files from the desired file folder
            var files = dir.GetFiles();

            ViewBag.path = ideaFolder;
            ViewBag.files = files;

            return View(idea);
        }

        // POST: Ideas/Approve/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(idea).State = EntityState.Modified;
                var currentIdea = db.Ideas.FirstOrDefault(p => p.ID == idea.ID);
                if (currentIdea == null)
                {
                    return HttpNotFound();
                }

                currentIdea.statusCode = idea.statusCode;
                currentIdea.denialReason = idea.denialReason;

                db.SaveChanges();

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the file storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var ideaFolder = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);
                DirectoryInfo dir = new DirectoryInfo(ideaFolder);

                // Store the files from the desired file folder
                var files = dir.GetFiles();

                ViewBag.path = ideaFolder;
                ViewBag.files = files;

                string body;
                if (idea.statusCode == "Approved")
                {
                    body = string.Format(
                        "Your idea was approved" +
                        "<br/><br/>{0}"
                        , idea.body);
                }
                else
                {
                    body = string.Format(
                        "Your idea wsa not approved" +
                        "<br/><br/>{0}" +
                        "<br/><br/>Reason for Denial:" +
                        "<br/>{1}" +
                        "<br/><br/>If this is not rectified in 10 business days," +
                        "the submission will be removed and no further action will be taken." +
                        "<br/><br/>Please go to <a href=\"http://localhost:52398/Ideas/Edit/{2}\">Great Ideas</a> to resubmit your idea."
                        , idea.body, idea.denialReason, idea.ID);
                }

                string subject = string.Format("New Idea Submission: {0}", idea.title);
                MailAddress from = new MailAddress("teamzed@outlook.com");
                MailAddress to = new MailAddress("rws10@live.com");


                //SendEmail(from, to, subject, body);
                return RedirectToAction("Index");
            }

            return View(idea);
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