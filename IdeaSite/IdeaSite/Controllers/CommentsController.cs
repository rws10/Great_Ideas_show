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
            smt.Send(msg);
        }

        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Comments
        public ActionResult Index(Idea idea)
        {
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int id)
        {
            ViewBag.ideaID = id;
            ViewBag.idea = db.Ideas.Where(x => x.ID == id).ToList();
            return View();
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
                comment.ideaID = id;
                comment.cre_date = DateTime.Now;
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

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
                //return RedirectToAction("Index/"+comment.ownerID);
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }
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
