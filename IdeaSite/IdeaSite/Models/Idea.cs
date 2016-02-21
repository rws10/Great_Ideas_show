using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaSite.Models
{
    public class Idea
    {
        public int ideaID { get; set; }

        // Name of the idea
        public string name { get; set; }

        // What is the idea?
        public string description { get; set; }

        // What is the Category of this idea?
        public string category { get; set; }

        // What is the date the idea was created?
        public DateTime creationDate { get; set; }

        // What is the name of the individual who submitted this idea?
        public string submitter { get; set; }

        // Is the idea Submitted, Active, or Archived?
        public string statusCode { get; set; }

        // Why was this idea Denied?
        public string denialReason { get; set; }

        // The attachments on this idea
        //public List<Attachment> attachments { get; set; }
    }
}