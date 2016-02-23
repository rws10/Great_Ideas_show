using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdeaSite.Models
{
    public class Idea
    {
        public int ideaID { get; set; }

        // Name of the idea
        [Display(Name = "Name")]
        public string name { get; set; }

        // What is the idea?
        [Display(Name = "Description")]
        public string description { get; set; }

        // What is the Category of this idea?
        [Display(Name = "Category")]
        public string category { get; set; }

        // What is the date the idea was created?
        [Display(Name = "Creation Date")]
        public DateTime creationDate { get; set; }

        // What is the name of the individual who submitted this idea?
        //[Display(Name = "Submitter")]
        //public string submitter { get; set; }

        // Is the idea Submitted, Active, or Archived?
        [Display(Name = "Status")]
        public string statusCode { get; set; }

        // Why was this idea Denied?
        [Display(Name = "Reason")]
        public string denialReason { get; set; }

        // The attachments on this idea
        //[Display(Name = "Attachments")]
        //public List<Attachment> attachments { get; set; }
    }
}