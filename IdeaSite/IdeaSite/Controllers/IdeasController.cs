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

        //home index
        public ActionResult Home()
        {
            return View();
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
            //searchTerms = search.Trim().Split(' ');
            searchTerms = SeparateSearchTerms(search);
            // new function before ^^ to find terms from string & phrases

            finalResults = results;
            for (int i = 0; i < searchTerms.Length; ++i)
            {
                // set to finalResults.Where (results => whatever) to allow a less strict searchs
                var term = searchTerms[i];
                if (searchBy == "Title") { finalResults = finalResults.Where(x => x.title.Contains(term)).ToList(); }
                else if (searchBy == "Description") { finalResults = finalResults.Where(x => x.body.Contains(term)).ToList(); }
                else if (searchBy == "All")
                {
                    finalResults = finalResults.Where(x => x.title.Contains(term) || x.body.Contains(term)).ToList();
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
            //if (search != null && search != "") { results = results.Concat(SearchByTerms(results, searchBy, search)); /*results = results.Distinct();*/ }
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
                

                var ideas = db.Ideas.Where(IDEA => IDEA.title == idea.title).ToList();

                if (ideas.Count > 0)
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

                db.Ideas.Add(idea);
                db.SaveChanges();

                // Compose an email to send to PPMO Group
                List<string> emailInfo = new List<string> { "1", idea.title, idea.body, idea.cre_user, idea.ID.ToString()};
                TempData["EmailInfo"] = emailInfo;
                return RedirectToAction("AutoEmail", "Mails");
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
                    var editorViewModel = new SelectFileEditorViewModel()
                    {
                        ID = attachment.ID,
                        Name = attachment.name,
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

            var tempModel = TempData["Model"] as FileSelectionViewModel;
            
            // Test to see if there was a redirection because of a duplicate title
            if (tempModel != null) {
                model = tempModel;
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

                var ideas = db.Ideas.Where(IDEA => IDEA.title == model.idea.title).ToList();

                if (ideas.Count > 0)
                {
                    TempData["Model"] = model;
                    TempData["Message"] = "Title must be a unique value";
                    return View(model);
                }

                currentIdea.title = model.idea.title;
                currentIdea.body = model.idea.body;
                currentIdea.statusCode = model.idea.statusCode;
                currentIdea.statusCodes = model.idea.statusCodes;
                currentIdea.denialReason = model.idea.denialReason;

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

                db.SaveChanges();

                List<string> emailInfo = new List<string> { "2", model.idea.title, model.idea.body, model.idea.cre_user, model.idea.ID.ToString()};

                // Compose an email to send to PPMO Group and return to index
                TempData["EmailInfo"] = emailInfo;
                return RedirectToAction("AutoEmail", "Mails");
            }
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

            TempData["Message"] = "The idea was successfully deleted.";
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

                /*var appSettings = ConfigurationManager.AppSettings;

                // store path to server location of the attachment storage
                var connectionInfo = appSettings["serverPath"];

                // combine the server location and the name of the new folder to be created
                var ideaFolder = string.Format(@"{0}{1}_{2}", connectionInfo, idea.ID, idea.title);
                DirectoryInfo dir = new DirectoryInfo(ideaFolder);

                // Store the attachments from the desired attachment folder
                var attachments = dir.GetFiles();

                //ViewBag.path = ideaFolder;
                //ViewBag.attachments = attachments;*/

                List<string> emailInfo;

                // Prepare email based on s
                if (idea.statusCode == "Accepted")
                {
                    emailInfo = new List<string> { "3", idea.title, idea.body, idea.cre_user};
                }
                else
                {
                    emailInfo = new List<string> { "4", idea.title, idea.body, idea.cre_user, idea.denialReason, idea.ID.ToString()};
                }

                // Compose an email to send to PPMO Group and return to index
                TempData["EmailInfo"] = emailInfo;
                return RedirectToAction("AutoEmail", "Mails");
            }

            return View(idea);
        }

        // GET: Ideas/Create

        /*public ActionResult SupportEmail()
        {
            return View();
        }*/

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