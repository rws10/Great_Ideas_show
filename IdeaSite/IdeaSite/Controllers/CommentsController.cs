using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdeaSite.Models;

// for commit
namespace IdeaSite.Controllers
{
    public class CommentsController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Comments
        public ActionResult Index(Idea idea)
        {
            List<Comment> comments = db.Comments.Where(com => com.ideaID == idea.ID).ToList();
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
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

                return RedirectToAction("Index", "Comments", idea);
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
                //comment.creationDate = DateTime.Now;
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
            return RedirectToAction("Index");
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
