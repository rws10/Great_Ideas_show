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
        [Key]
        [Required]
        public int ID { get; set; }

        // Name of the idea
        [Required]
        [Display(Name = "Title")]        
        public string title { get; set; }

        // What is the idea?
        [Required(ErrorMessage = "Description Required")]
        [Display(Name = "Description")]
        public string body { get; set; }

        // What is the date the idea was created?
        // This needs to be set automatically when the form is submitted
        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime cre_date { get; set; }

        // What is the name of the individual who submitted this idea?
        [Display(Name = "Name")]
        public string cre_user { get; set; }

        // Is the idea Submitted, Active, Rejected, or Archived?
        // This needs to be adjusted as the idea's status changes. n
        // On submission it needs to be "Submitted"
        // On approval it needs to be "Added"
        // On archival it needs to be "Archived"
        // On rejection it needs to be "Denied"
        [Required]
        [Display(Name = "Status")]
        public string statusCode { get; set; }

        // Why was this idea Denied?
        // This needs to be a conditional display of only when the status is set to "Rejected"
        [Display(Name = "Reason for Decision")]
        public string denialReason { get; set; }

        //public List<Comment> comments { get; set; }
    }
}