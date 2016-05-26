using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdeaSite.Models;
using System.Net.Mail;

// for commit
namespace IdeaSite.Controllers
{
    public class CommentsController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Comments
        public ActionResult Index(int id)
        {
            Idea idea = db.Ideas.Find(id);
            IEnumerable<Comment> comments = new List<Comment>();
            //List<Comment> comments = db.Comments.Where(com => com.ideaID == idea.ID).ToList();
            comments = db.Comments.Where(com => com.ideaID == idea.ID).ToList();
            comments = comments.Reverse();
            ViewBag.idea = idea;
            return View(comments);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int id)
        {
            ViewBag.ideaID = id;

            // pull the current user's name from active directory to use it for cre_user
            //System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            //string[] a = HttpContext.User.Identity.Name.Split('\\');
            //System.DirectoryServices.DirectoryEntry ADEntry = new System.DirectoryServices.DirectoryEntry("WinNT://" + a[0] + "/" + a[1]);
            //string name = ADEntry.Properties["FullName"].Value.ToString();
            string name = "Administrator";
            Comment comment = new Comment
            {
                ideaID = id,
                cre_user = name,
                cre_date = DateTime.Now
            };

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ideaID,cre_user,body,cre_date,")] Comment comment, int id)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();

                Idea idea = db.Ideas.Find(comment.ideaID);

                List<string> emailInfo = new List<string> { "5", idea.title, idea.body, idea.cre_user, idea.ID.ToString()};
                TempData["EmailInfo"] = emailInfo;
                TempData["IdeaID"] = idea.ID;

                return RedirectToAction("AutoEmail", "Mails");
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ideaID,cre_user,body,cre_date,")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.cre_date = DateTime.Now;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                Idea idea = db.Ideas.Find(comment.ideaID);

                return RedirectToAction("Index", idea);
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();

            Idea idea = db.Ideas.Find(comment.ideaID);

            return RedirectToAction("Index", idea);
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
