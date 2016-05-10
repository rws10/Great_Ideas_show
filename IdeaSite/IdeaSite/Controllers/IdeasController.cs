using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdeaSite.Models;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Data.Entity;

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
            smt.Credentials = new NetworkCredential("teamzed@outlook.com", "T3@m_Z3d");
            smt.EnableSsl = true;
            // emails disabled because there is no error handling for a bad connection.
            //smt.Send(msg);
        }

        //home index
        public ActionResult Home()
        {
            return View();
        }


        private string[] SeparateSearchTerms(string search)
        {
            List<String> holdKeywords = new List<String>(search.Trim().Split(' '));
            bool flag = false;
            foreach (String keyword in holdKeywords.ToList())
            {
                if (keyword.Contains("\"") && !flag) { holdKeywords.Remove(keyword); flag = true; }
                else if (keyword.Contains("\"") && flag) { holdKeywords.Remove(keyword); flag = false; }
                else if (flag) { holdKeywords.Remove(keyword); }
            }


            List<String> searchTermsList = new List<String>();
            for (int i = 0; i < search.Length; ++i)
            {
                searchTermsList.Add(search[i].ToString());
            }
            String phrase = "";
            List<String> phraseL = new List<String>();
            Queue<String> phraseQ = new Queue<String>();
            List<String> searchPhraseList = new List<String>();
            //bool flag = false;
            flag = false;
            foreach (String term in searchTermsList)
            {
                if (term == "\"" || flag == true)
                {
                    flag = true;
                    phraseQ.Enqueue(term);
                    if (term == "\"" && phraseQ.Count > 2)
                    {
                        phraseQ.Dequeue();
                        while (phraseQ.Peek() != "\"") { phraseL.Add(phraseQ.Dequeue()); }
                        phrase = string.Join("", phraseL.ToArray());
                        phraseQ.Clear();
                        phraseL.Clear();
                        flag = false;
                    }
                    //else { searchPhraseList.Add(phrase); }   
                    //if (phraseQ.Count() == 0) { searchPhraseList.Add(phrase); }
                    if (!flag) { searchPhraseList.Add(phrase); }
                }
                else
                {
                    searchPhraseList.Add(term);
                }
            }
            String[] searchTerms = new String[searchPhraseList.Count()];
            int pos = 0;
            foreach (String term in searchPhraseList) { searchTerms[pos++] = term; }
            var removeSpaces = new List<String>(searchTerms);
            removeSpaces.Remove(" ");
            removeSpaces = removeSpaces.Concat(holdKeywords).ToList();
            searchTerms = removeSpaces.ToArray();
            return searchTerms;
        }


        private IEnumerable<Idea> SearchByTerms(IEnumerable<Idea> _results, string searchBy, string search)
        {
            //IEnumerable<Idea> results = new List<Idea>();
            IEnumerable<Idea> results = _results;
            IEnumerable<Idea> finalResults = new List<Idea>();
            string[] searchTerms;
            //searchTerms = search.Trim().Split(' ');
            searchTerms = SeparateSearchTerms(search);
            // new function before ^^ to find terms from string & phrases
            for (int i = 0; i < searchTerms.Length; ++i)
            {
                var term = searchTerms[i];
                if (searchBy == "Title") { finalResults = results.Where(x => x.title.Contains(term)).ToList(); }
                else if (searchBy == "Description") { finalResults = results.Where(x => x.body.Contains(term)).ToList(); }
                else if (searchBy == "All")
                {
                    finalResults = results.Where(x => x.title.Contains(term)).ToList();
                    finalResults = finalResults.Concat(results.Where(x => x.body.Contains(term)).ToList());
                }
            }
            return MatchSearchResults(finalResults);
        }

        private IEnumerable<Idea> MatchSearchResults(IEnumerable<Idea> results)
        {
            var ideas = db.Ideas.ToList();
            List<int[]> matches = new List<int[]>();
            foreach (Idea idea in results)
            {
                foreach (var match in matches) { if (idea.ID == match[0]) { ++match[1]; } }
            }
            foreach (Idea idea in ideas)
            {
                int[] match = new int[2];
                match[0] = idea.ID;
                match[1] = 0;
                matches.Add(match);
            }
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
                        finalResults = finalResults.Concat(results.Where(x => x.ID == idea.ID).ToList());
                    }
                }
            }
            finalResults = finalResults.Distinct();
            return finalResults;
        }

        // GET: Ideas
        public ActionResult Index(string searchBy, string search, string[] sortByStatus)
        {
            IEnumerable<Idea> results = new List<Idea>();
            // This handles the first run of the index. Since the default checkbox is set to Accepted,
            // The default (first run with parameters of null) view is to show Accepted ideas
            // Checkboxes will maintain their "checked" status across searches
            if (searchBy == null && search == null && sortByStatus == null)
            {
                results = results.Concat(db.Ideas.Where(x => x.statusCode == "Accepted"));
                results = results.Reverse();
                return View(results);
            }
            if (sortByStatus[0] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Submitted")); }
            if (sortByStatus[1] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Accepted")); }
            if (sortByStatus[2] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Denied")); }
            if (search != null && search != "") { results = SearchByTerms(results, searchBy, search); }
            else { results = results.Reverse(); } // the search above already reverses the results
            return View(results);
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

            return View(idea);
        }

        // GET: Ideas/Create

        public ActionResult Create()
        {
            Idea tempIdea = TempData["Idea"] as Idea;
            return View(tempIdea);
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

                try
                {
                    db.SaveChanges();
                }

                catch
                {
                    TempData["Idea"] = idea;
                    TempData["Message"] = "Title must be a unique value";
                    return View(idea);
                }

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the attachment storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);

                DirectoryInfo di = Directory.CreateDirectory(storagePath);
                if (files != null)
                {
                    try
                    {
                        // loop through the uploads and pull out each attachment from it.
                        for (int i = 0; i < files.Count(); ++i)
                        {
                            if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                            {
                                // store the name of the attachment
                                var attachmentName = Path.GetFileName(files.ElementAt(i).FileName);

                                // create new object to reference the loaction of the new attachment and the ID of the idea to which it belongs.
                                var attachment = new Models.Attachment
                                {
                                    storageLocation = string.Format("{0}\\", storagePath),
                                    name = attachmentName,
                                    cre_date = DateTime.Now,
                                    ownIdea = idea,
                                    delete = false
                                };

                                files.ElementAt(i).SaveAs(string.Format("{0}\\{1}", storagePath, attachmentName));

                                if (idea.attachments == null)
                                {
                                    idea.attachments = new List<Models.Attachment>();
                                }
                                idea.attachments.Add(attachment);
                                db.Attachments.Add(attachment);
                                db.SaveChanges();
                            }
                        }
                    }

                    catch
                    {
                        TempData["Idea"] = idea;
                        TempData["Message"] = "One or more attachments failed to upload";
                        return RedirectToAction("Create");
                    }
                }

                // Compose an email to send to PPMO Group
                string subject = string.Format("New Idea Submission: {0}", idea.title);

                string body = string.Format("{0} has submitted an Idea on Great Ideas:" +
                    "<br/><br/>{1}:" +
                    "<br/>{2}" +
                    "<br/><br/>Please go to <a href=\"http://localhost:52398/Ideas/Approval/{2}\">Great Ideas</a> to submit approval.",
                    idea.cre_user, idea.title, idea.body);

                MailAddress from = new MailAddress("teamzed@outlook.com");
                MailAddress to = new MailAddress("rws10@live.com");


                SendEmail(from, to, subject, body);

                TempData["Message"] = "Your idea has been successfully created.";
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

            var model = new FileSelectionViewModel();

            model.idea = db.Ideas.Find(id);
            model.idea.statusCode = "Submitted";

            foreach (var attachment in db.Attachments)
            {
                if (attachment.IdeaID == model.idea.ID)
                {
                    var editorViewModel = new FileSelectorEditorViewModel()
                    {
                        ID = attachment.ID,
                        Name = string.Format("{0}\\{1}", attachment.storageLocation, attachment.name),
                        Selected = false
                    };
                    model.Attachs.Add(editorViewModel);
                }
            }

            /*idea.statusCodes = new[]
            {
                new SelectListItem { Value = "Added", Text = "Added" },
                new SelectListItem { Value = "Archived", Text = "Archived" },
                new SelectListItem { Value = "Project Submission", Text = "Project Submission" },
            };*/

            if (model.idea == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FileSelectionViewModel model, IEnumerable<HttpPostedFileBase> attachments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model.idea).State = EntityState.Modified;
                var currentIdea = db.Ideas.FirstOrDefault(p => p.ID == model.idea.ID);
                if (currentIdea == null)
                {
                    return HttpNotFound();
                }

                currentIdea.title = model.idea.title;
                currentIdea.body = model.idea.body;
                currentIdea.statusCode = model.idea.statusCode;
                currentIdea.statusCodes = model.idea.statusCodes;
                currentIdea.denialReason = model.idea.denialReason;

                

                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    TempData["Idea"] = model.idea;
                    TempData["Message"] = "Title must be a unique value";
                    return View(model.idea);
                }

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
                    attachment.DeleteFile();
                }

                string subject = string.Format("An idea has been edited: {0}", model.idea.title);

                string body = string.Format("{0} has Edited an Idea on Great Ideas:" +
                    "<br/><br/>{1}:" +
                    "<br/>{2}" +
                    "<br/><br/>Please go to <a href=\"http://localhost:52398/Ideas/Approval/{3}\">Great Ideas</a> to submit approval.",
                    model.idea.cre_user, model.idea.title, model.idea.body, model.idea.ID);

                MailAddress from = new MailAddress("teamzed@outlook.com");
                MailAddress to = new MailAddress("rws10@live.com");


                SendEmail(from, to, subject, body);

                var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the attachment storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, model.idea.ID, model.idea.title);

                if (!Directory.Exists(storagePath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(storagePath);
                }

                try
                {
                    // loop through the uploads and pull out each attachment from it.
                    for (int i = 0; i < attachments.Count(); ++i)
                    {
                        if (attachments.ElementAt(i) != null && attachments.ElementAt(i).ContentLength > 0)
                        {
                            // store the name of the attachment
                            var attachmentName = Path.GetFileName(attachments.ElementAt(i).FileName);

                            // create new object to reference the loaction of the new attachment and the ID of the idea to which it belongs.
                            var attachment = new Models.Attachment
                            {
                                storageLocation = string.Format("{0}\\", storagePath),
                                name = attachmentName,
                                cre_date = DateTime.Now,
                                ownIdea = model.idea,
                                delete = false
                            };

                            attachments.ElementAt(i).SaveAs(string.Format("{0}\\{1}", storagePath, attachmentName));

                            if (model.idea.attachments == null)
                            {
                                model.idea.attachments = new List<Models.Attachment>();
                            }
                            model.idea.attachments.Add(attachment);
                            db.Attachments.Add(attachment);
                        }
                    }
                }

                catch(Exception ex)
                {
                    TempData["Message"] = "The attachments failed to upload";
                    return RedirectToAction("Edit");
                }

                db.SaveChanges();
                TempData["Message"] = "Your idea has been successfully submitted.";
                return RedirectToAction("Index");
            }
            //ViewBag.attachments = attachments;
            return View(model);
        }

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

            /* Create a list of the attachments associated with the idea to allow the deletion of the files 
             * associated with them. Once all of the files are gone from each files directory, the directory is 
             * deleted*/
            IEnumerable<Models.Attachment> attachments = new List<Models.Attachment>();
            attachments = db.Attachments.Where(attach => attach.ownIdea.ID == idea.ID).ToList();

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    attachment.DeleteFile();
                    attachment.DeleteDirectory();
                }

                db.Attachments.RemoveRange(db.Attachments.Where(attach => attach.ownIdea.ID == id));
            }

            db.Comments.RemoveRange(db.Comments.Where(com => com.ideaID == id));

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

                // store path to server location of the attachment storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var ideaFolder = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);
                DirectoryInfo dir = new DirectoryInfo(ideaFolder);

                // Store the attachments from the desired attachment folder
                var attachments = dir.GetFiles();

                ViewBag.path = ideaFolder;
                ViewBag.attachments = attachments;

                string body;
                if (idea.statusCode == "Added")
                {
                    body = string.Format(
                        "Your idea was added" +
                        "<br/><br/>{0}"
                        , idea.body);
                }
                else
                {
                    body = string.Format(
                        "Your idea wsa not added" +
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


                SendEmail(from, to, subject, body);
                return RedirectToAction("Index");
            }

            return View(idea);
        }

        public FileResult Download(string attachmentpath, string attachmentName)
        {
            return File(attachmentpath, System.Net.Mime.MediaTypeNames.Application.Octet, attachmentName);
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