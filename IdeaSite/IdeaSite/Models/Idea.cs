using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        // What is the date the idea was created?
        // This needs to be set automatically when the form is submitted
        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime creationDate { get; set; }

        // What is the name of the individual who submitted this idea?
        //[Display(Name = "Submitter")]
        //public string submitter { get; set; }

        // Is the idea Submitted, Active, Rejected, or Archived?
        // This needs to be adjusted as the idea's status changes. 
        // On submission it needs to be "Submitted"
        // On approval it needs to be "Approved"
        // On archival it needs to be "Archived"
        // On rejection it needs to be "Rejected"
        [Display(Name = "Status")]
        public string statusCode { get; set; }

        // Why was this idea Denied?
        // This needs to be a conditional display of only when the status is set to "Rejected"
        [Display(Name = "Reason for Denial")]
        public string denialReason { get; set; }

        // The attachments on this idea
        //[Display(Name = "Attachments")]
        //public List<Attachment> attachments { get; set; }

        //public List<Comment> comments { get; set; }
    }
}