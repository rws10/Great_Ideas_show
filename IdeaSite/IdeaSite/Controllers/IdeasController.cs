using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdeaSite.Models;
using System.Configuration;
using System.IO;
using System.Data.Entity;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using X.PagedList;
using log4net;
using System.Data.SqlClient;

namespace IdeaSite.Controllers
{
    //[Authorize]
    public class IdeasController : Controller
    {
        private static readonly ILog log = LogHelper.GetLogger();

        private IdeaSiteContext db = new IdeaSiteContext();

        //home index
        public ActionResult Home()
        {
            return View();
        }

        // GET: Ideas
        public ActionResult Index(int? page, string currentSearch, string sortByStatus, string[] sortByStatusArr, string search, string searchBy)
        {
            int pageSize = 10;

            ViewBag.searchBy = searchBy;

            ViewBag.sortByStatusArr = sortByStatusArr;
            ViewBag.sortByStatus = sortByStatus;

            var roles = GetRolesForUser(GetUsername(0));

            // get the name of the current user
            ViewBag.currentUser = GetUsername(1);
            var currentUser = GetUsername(1);

            // get the domain name for the admin group
            var appSettings = ConfigurationManager.AppSettings;
            string group = appSettings["AdminGroup"];

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentSearch;
            }

            ViewBag.currentSearch = search;

            // determine if the user is an admin and carry this information to the view
            ViewBag.isAdmin = false;
            for (int i = 0; i < roles.Count(); i++)
            {
                if (roles.ElementAt(i) == group)
                {
                    ViewBag.isAdmin = true;
                    break;
                }
            }

            int pageNumber = (page ?? 1);

            IEnumerable<Idea> results = new List<Idea>();

            // This handles the first run of the index. Since the default checkbox is set to Accepted,
            // The default (first run with parameters of null) view is to show Accepted ideas
            // Checkboxes will maintain their "checked" status across searches
            if (searchBy == null && search == null && sortByStatus == null)
            {
                results = results.Concat(db.Ideas.Where(x => x.statusCode == "Accepted"));
                results = results.Reverse().ToPagedList(pageNumber, pageSize);
                ViewBag.OnePageOfIdeas = results;
                //return View(results);
                return View(results.ToPagedList(pageNumber, pageSize));
            }

            if (sortByStatus != null)
            {
                string[] stringSeparators = new string[] { " " };
                sortByStatusArr = sortByStatus.Split(stringSeparators, StringSplitOptions.None);

            }
            else
            {
                sortByStatus = string.Join(" ", sortByStatusArr);
            }

            ViewBag.sortByStatus = sortByStatus;
            try {
                if (sortByStatusArr[0] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Submitted")); }
                if (sortByStatusArr[1] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Accepted")); }
                if (sortByStatusArr[2] == "true")
                {
                    if (ViewBag.isAdmin)
                    {
                        results = results.Concat(db.Ideas.Where(x => x.statusCode == "Denied"));
                    }
                    else
                    {
                        results = results.Concat(db.Ideas.Where(x => x.statusCode == "Denied" && x.cre_user == currentUser));
                    }
                }

                if (sortByStatusArr[3] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Archive")); }
                if (sortByStatusArr[4] == "true") { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Active Project")); }
                if (sortByStatusArr[5] == "true" && sortByStatusArr[0] == "false")
                { results = results.Concat(db.Ideas.Where(x => x.statusCode == "Submitted" && x.cre_user == currentUser)); }
            }
            catch(SqlException ex)
            {
                log.Error("An error has occured while accessing the database.", ex);
                return RedirectToAction("Index", "Error");
            }

            if (search != null && search != "") { results = SearchByTerms(results, searchBy, search); }
            else { results = results.Reverse(); } // the search above already reverses the results

            results = results.ToPagedList(pageNumber, pageSize);
            ViewBag.OnePageOfIdeas = results;

            return View(results);
        }

        // GET: Ideas/Details/
        public ActionResult Details(int? id)
        {
            Idea idea = db.Ideas.Find(id);

            ViewBag.currentUser = GetUsername(1);

            return View(idea);
        }

        // GET: Ideas/Submit

        public ActionResult Submit()
        {
            return View();
        }

        // POST: Ideas/Submit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason, mod_date, email, commentsNumber")] Idea idea, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                idea.cre_user = GetUsername(1);
                //idea.cre_user = "Administrator";
                idea.cre_date = DateTime.Now;
                idea.mod_date = DateTime.Now;
                idea.email = GetEmail();
                var ideas = db.Ideas.Where(IDEA => IDEA.title == idea.title).ToList();

                if (ideas.Count > 0)
                {
                    TempData["FailureMessage"] = "Title must be a unique value";
                    return View(idea);
                }

                if (files != null)
                {
                    var appSettings = ConfigurationManager.AppSettings;

                    // store path to server location of the attachment storage
                    var connectionInfo = appSettings["serverPath"];

                    // combine the server location and the name of the new folder to be created
                    var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);

                    // create the folder for the idea's attachments
                    DirectoryInfo di = Directory.CreateDirectory(storagePath);

                    // run each file through MetaScan and check for forbidden extensions
                    for (int i = 0; i < files.Count(); ++i)
                    {
                        if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                        {
                            var file = files.ElementAt(i);
                            // store the name of the attachment

                            // place the file into a bytearray for MetaScan use
                            FileStream stream = System.IO.File.OpenRead(Path.GetFullPath(file.FileName));
                            byte[] fileBytes = new byte[stream.Length];

                            stream.Read(fileBytes, 0, fileBytes.Length);
                            stream.Close();

                            // make the file a bytearray and send to MetaScan here?
                            FileService.FileService sf = new FileService.FileService();

                            string ext = Path.GetExtension(file.FileName);

                            bool valrtn = sf.ScanByFileWithNameAndExtension(fileBytes, file.FileName, ext);

                            // If any file fails to load, reload the creation screen and inform the submitter of the need to scan the files.
                            if (!valrtn)
                            {
                                TempData["FailureMessage"] += "\n\nThe files failed to upload. " +
                                    "Please scan them locally using McAffee before uploading them again.";

                                return View(idea);
                            }
                        }
                    }

                    db.Ideas.Add(idea);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (SqlException ex)
                    {
                        log.Error("An error has occured while accessing the database.", ex);
                        return RedirectToAction("Index", "Error");
                    }

                    // save the files to the specified folder and link them to the idea
                    for (int i = 0; i < files.Count(); ++i)
                    {
                        if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                        {
                            var file = files.ElementAt(i);
                            var attachmentName = Path.GetFileName(file.FileName);
                            var namepath = string.Format("{0}\\{1}", storagePath, attachmentName);

                            // save the new Attachments to the Idea
                            var attachment = new Models.Attachment
                            {
                                storageLocation = string.Format("{0}\\", storagePath),
                                name = attachmentName,
                                cre_date = DateTime.Now,
                                deleteObj = "false"
                            };

                            try
                            {
                                file.SaveAs(string.Format("{0}\\{1}", storagePath, attachmentName));
                            }
                            catch (Exception ex)
                            {
                                log.Error("An error has occured while attempting to save a file.", ex);
                                return RedirectToAction("Index", "Error");
                            }

                            if (idea.attachments == null)
                            {
                                idea.attachments = new List<Models.Attachment>();
                            }

                            db.Attachments.Add(attachment);

                            try
                            {
                                db.SaveChanges();
                            }
                            catch (SqlException ex)
                            {
                                log.Error("There was an issue saving changes to the database.", ex);
                                return RedirectToAction("Index", "Error");
                            }
                        }
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }


                // Compose an email to send to PPMO Group
                List<string> emailInfo = new List<string> { "1", idea.email, idea.title, idea.body, idea.cre_user, idea.ID.ToString() };
                TempData["EmailInfo"] = emailInfo;
                return RedirectToAction("AutoEmail", "Mails");

                // This is only for Josh and Alex since they don't have access to AD
                //return RedirectToAction("Index", "Ideas");
            }
            return View(idea);
        }

        // GET: Ideas/Edit/5
        public ActionResult Edit(int? id)
        {
            var model = new FileSelectionViewModel();

            model.idea = db.Ideas.Find(id);

            foreach (var attachment in db.Attachments)
            {
                if (attachment.ideaID == model.idea.ID)
                {
                    var editorViewModel = new SelectFileEditorViewModel()
                    {
                        ID = attachment.ID,
                        Name = attachment.name,
                        Selected = false
                    };
                    model.Attachs.Add(editorViewModel);
                }
            }
            return View(model);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FileSelectionViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model.idea).State = EntityState.Modified;

                var currentIdea = db.Ideas.FirstOrDefault(p => p.ID == model.idea.ID);

                var ideas = db.Ideas.Where(IDEA => IDEA.title == model.idea.title && IDEA.ID != model.idea.ID).ToList();

                if (ideas.Count > 0)
                {
                    return View(model);
                }

                // If the Idea is already in the "Submitted" state, another email will not be sent.
                bool alreadySubmitted = false;
                if (currentIdea.statusCode == "Submitted")
                {
                    alreadySubmitted = true;
                }
                currentIdea.title = model.idea.title;
                currentIdea.body = model.idea.body;
                currentIdea.statusCode = "Submitted";
                currentIdea.denialReason = model.idea.denialReason;
                currentIdea.mod_date = DateTime.Now;
                currentIdea.email = model.idea.email;

                if (files != null)
                {
                    var appSettings = ConfigurationManager.AppSettings;

                    // store path to server location of the attachment storage
                    var connectionInfo = appSettings["serverPath"];

                    // combine the server location and the name of the new folder to be created
                    var storagePath = string.Format(@"{0}{1}_{2}", connectionInfo, model.idea.ID, model.idea.title);
                    if (!Directory.Exists(storagePath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(storagePath);
                    }

                    //run each file through MetaScan and check for forbidden extensions
                    for (int i = 0; i < files.Count(); ++i)
                    {
                        if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                        {
                            var file = files.ElementAt(i);
                            // store the name of the attachment

                            // place the file into a bytearray for MetaScan use
                            FileStream stream = System.IO.File.OpenRead(Path.GetFullPath(file.FileName));
                            byte[] fileBytes = new byte[stream.Length];

                            stream.Read(fileBytes, 0, fileBytes.Length);
                            stream.Close();

                            // make the file a bytearray and send to MetaScan here?
                            FileService.FileService sf = new FileService.FileService();

                            string ext = Path.GetExtension(file.FileName);

                            bool valrtn = sf.ScanByFileWithNameAndExtension(fileBytes, file.FileName, ext);

                            // If any file fails to load, reload the creation screen and inform the submitter of the need to scan the files.
                            if (!valrtn)
                            {
                                TempData["ScanFailure"] = "\n\nThe files failed to upload. " +
                                    "Please scan them locally using McAffee before uploading them again." +
                                    " Selected files have not been deleted.";

                                return View(model);
                            }
                        }
                    }

                    // save the files to the specified folder and link them to the idea
                    for (int i = 0; i < files.Count(); ++i)
                    {
                        if (files.ElementAt(i) != null && files.ElementAt(i).ContentLength > 0)
                        {
                            var file = files.ElementAt(i);
                            var attachmentName = Path.GetFileName(file.FileName);
                            var namepath = string.Format("{0}\\{1}", storagePath, attachmentName);

                            // save the new Attachments to the Idea
                            var attachment = new Models.Attachment
                            {
                                storageLocation = string.Format("{0}\\", storagePath),
                                name = attachmentName,
                                cre_date = DateTime.Now,
                                deleteObj = "false"
                            };

                            try
                            {
                                file.SaveAs(string.Format("{0}\\{1}", storagePath, attachmentName));
                            }
                            catch (Exception ex)
                            {
                                TempData["Message"] += String.Format("An error was caught: Error number: {0}, Message: {1}", ex.HResult, ex.Message);
                                return View(model);
                            }

                            if (model.idea.attachments == null)
                            {
                                model.idea.attachments = new List<Models.Attachment>();
                            }

                            model.idea.attachments.Add(attachment);
                            db.Attachments.Add(attachment);
                        }
                    }
                }
                // save the new Attachments to the Idea
                try
                {
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
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
                    db.Attachments.Remove(attachment);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }

                if (!alreadySubmitted)
                {
                    // Compose an email to send to PPMO Group and return to index
                    List<string> emailInfo = new List<string> { "2", model.idea.email, model.idea.title, model.idea.body, model.idea.cre_user, model.idea.ID.ToString() };
                    TempData["EmailInfo"] = emailInfo;

                    return RedirectToAction("AutoEmail", "Mails");
                    // This is only for Josh and Alex since they don't have access to AD
                    //return RedirectToAction("Index", "Ideas");
                }
                TempData["Message"] = "Your idea has been successfully submitted.";
                return RedirectToAction("Index", "Ideas");

            }
            return View(model);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            Idea idea = db.Ideas.Find(id);
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
            attachments = db.Attachments.Where(attach => attach.ideaID == idea.ID).ToList();

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    attachment.DeleteFile();
                    attachment.DeleteDirectory();
                }

                try
                {
                    db.Attachments.RemoveRange(db.Attachments.Where(attach => attach.ideaID == id));
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }
            }

            try
            {
                db.Comments.RemoveRange(db.Comments.Where(com => com.ideaID == id));

                db.Ideas.Remove(idea);

                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                log.Error("An error has occured while accessing the database.", ex);
                return RedirectToAction("Index", "Error");
            }

            TempData["Message"] = "The idea was successfully deleted.";
            return RedirectToAction("Index");
        }

        // GET: Ideas/Archive/5
        public ActionResult Archive(int? id)
        {
            Idea idea = null;
            try
            {
                idea = db.Ideas.Find(id);
            }
            catch (SqlException ex)
            {
                log.Error("An error has occured while accessing the database.", ex);
                return RedirectToAction("Index", "Error");
            }

            return View(idea);
        }

        // POST: Ideas/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public ActionResult ArchiveConfirmed([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason, mod_date, email, commentsNumber")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(idea).State = EntityState.Modified;

                if (idea.statusCode == "Archive")
                {
                    idea.statusCode = "Accepted";
                    TempData["Message"] = "The idea was successfully removed from archived list.";
                }
                else
                {
                    idea.statusCode = "Archive";
                    TempData["Message"] = "The idea was successfully archived.";
                }

                try
                {
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Ideas/ProjectSubmission/5
        public ActionResult ActiveProject(int? id)
        {
            Idea idea = db.Ideas.Find(id);
            idea.statusCode = "Active Project";

            return View(idea);
        }

        // This needs an autoemail
        // POST: Ideas/ProjectSubmission/5
        [HttpPost, ActionName("ActiveProject")]
        [ValidateAntiForgeryToken]
        public ActionResult ActiveProjectConfirmed([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason, mod_date, email, commentsNumber")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(idea).State = EntityState.Modified;

                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }

                TempData["SuccessMessage"] = "The idea was successfully moved to Active Project.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Ideas/Approve/5
        public ActionResult Approval(int? id)
        {
            Idea idea = db.Ideas.Find(id);
            if (idea.statusCode == "Submitted")
            {
                return View(idea);
            }
            else
            {
                TempData["FailureMessage"] = "An approval decision has already been made on this idea.";
                return RedirectToAction("Index");
            }

        }

        // POST: Ideas/Approve/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval([Bind(Include = "ID,title,body,cre_date,cre_user,statusCode,denialReason, mod_date, email, commentsNumber")] Idea idea)
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

                try
                {
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    log.Error("An error has occured while accessing the database.", ex);
                    return RedirectToAction("Index", "Error");
                }

                //ViewBag.attachments = attachments;*/

                List<string> emailInfo;

                // Prepare email based on s
                if (idea.statusCode == "Accepted")
                {
                    emailInfo = new List<string> { "3", idea.email, idea.title, idea.body, idea.cre_user };
                }
                else
                {
                    emailInfo = new List<string> { "4", idea.email, idea.title, idea.body, idea.cre_user, idea.denialReason, idea.ID.ToString() };
                }

                // Compose an email to send to PPMO Group and return to index
                TempData["EmailInfo"] = emailInfo;
                return RedirectToAction("AutoEmail", "Mails");

                // This is only for Josh and Alex since they don't have access to AD
                //return RedirectToAction("Index", "Ideas");
            }

            return View(idea);
        }

        private string[] SeparateSearchTerms(string search)
        {
            //List<String> holdKeywords = new List<String>(search.Trim().Split(' '));
            List<String> holdKeywords = new List<String>(search.Split(' '));
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
            //searchTindexerms = search.Trim().Split(' ');
            searchTerms = SeparateSearchTerms(search);
            // new function before ^^ to find terms from string & phrases

            finalResults = results;
            for (int i = 0; i < searchTerms.Length; ++i)
            {
                // set to finalResults.Where (results => whatever) to allow a less strict searchs
                var term = searchTerms[i].ToUpper();
                if (searchBy == "Title") { finalResults = finalResults.Where(x => x.title.ToUpper().Contains(term)).ToList(); }
                else if (searchBy == "Description") { finalResults = finalResults.Where(x => x.body.ToUpper().Contains(term)).ToList(); }
                else if (searchBy == "All")
                {
                    finalResults = finalResults.Where(x => x.title.ToUpper().Contains(term) || x.body.ToUpper().Contains(term)).ToList();
                    //finalResults = finalResults.Concat(finalResults.Where(x => x.body.Contains(term)).ToList());
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


        public string GetUsername(int i)
        {
            // pull the current user's name from active directory and use it for cre_user
            System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            string[] a = HttpContext.User.Identity.Name.Split('\\');
            DirectoryEntry ADEntry = new DirectoryEntry("WinNT://" + a[0] + "/" + a[1]);

            if (i == 0)
            {
                return ADEntry.Properties["Name"].Value.ToString();
            }
            return ADEntry.Properties["FullName"].Value.ToString();
        }

        public string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            string[] user = username.Split(new char[] { '@' });
            SearchResult result;
            DirectorySearcher search = new DirectorySearcher();
            search.Filter = String.Format("(SAMAccountName={0})", user[0]);
            // member contains list of users identified by distinguishedName
            search.PropertiesToLoad.Add("memberof");
            result = search.FindOne();
            if (result != null)
            {
                // search through members of group
                for (int counter = 0; counter < result.Properties["memberof"].Count; counter++)
                {
                    SearchResult srUser;
                    search = new DirectorySearcher();
                    // Filter on distinguishedName to find user
                    search.Filter = string.Format("(distinguishedName={0})", (string)result.Properties["memberof"][counter]);
                    // samaccountname is login id without domain qualifier
                    search.PropertiesToLoad.Add("SAMAccountName");
                    srUser = search.FindOne();
                    if (srUser != null)
                    {
                        roles.Add((string)srUser.Properties["samaccountname"][0].ToString());
                    }
                }
            }
            return roles.ToArray();
        }

        string GetEmail()
        {
            string username = Environment.UserName;
            string domain = Environment.UserDomainName;

            List<string> emailAddresses = new List<string>();

            PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(domainContext, username);

            // Add the "mail" entry
            emailAddresses.Add(user.EmailAddress);

            // Add the "proxyaddresses" entries.
            System.DirectoryServices.PropertyCollection properties = ((DirectoryEntry)user.GetUnderlyingObject()).Properties;
            foreach (object property in properties["proxyaddresses"])
            {
                emailAddresses.Add(property.ToString());
            }

            string from = null;

            for (int i = 0; i < emailAddresses.Count; i++)
            {
                if (emailAddresses[i].Contains("@freshfromflorida.com"))
                {
                    from = emailAddresses[i];
                    break;
                }
            }

            return from;
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