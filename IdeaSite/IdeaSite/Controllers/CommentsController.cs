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
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Configuration;
using PagedList;

// for commit
namespace IdeaSite.Controllers
{
    public class CommentsController : Controller
    {
        private IdeaSiteContext db = new IdeaSiteContext();

        // GET: Comments
        public ActionResult Index(int id, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;

            ViewBag.currentUser = GetUsername(1);
            var roles = GetRolesForUser(GetUsername(0));

            var appSettings = ConfigurationManager.AppSettings;
            string group = appSettings["AdminGroup"];

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

            Idea idea = db.Ideas.Find(id);
            IEnumerable<Comment> comments = new List<Comment>();
            //List<Comment> comments = db.Comments.Where(com => com.ideaID == idea.ID).ToList();
            comments = db.Comments.Where(com => com.ideaID == idea.ID).ToList();
            comments = comments.Reverse();
            var onePageofComments = comments.ToPagedList(pageNumber, pageSize);
            ViewBag.OnePageOfComments = onePageofComments;
            ViewBag.idea = idea;

            return View(onePageofComments);
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

            string name = GetUsername(1);
            //string name = "Administrator";
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
                Idea idea = db.Ideas.Find(comment.ideaID);

                ++idea.commentsNumber;

                db.SaveChanges();


                List<string> emailInfo = new List<string> { "5", idea.email, idea.title, idea.body, idea.cre_user, idea.ID.ToString()};
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

            Idea idea = db.Ideas.Find(comment.ideaID);

            --idea.commentsNumber;
            db.SaveChanges();

            return RedirectToAction("Index", idea);
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
