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
        [Required(ErrorMessage = "Title Required")]
        [Display(Name = "Title")]        
        public string title { get; set; }

        // What is the idea?
        [Required(ErrorMessage = "Description Required")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string body { get; set; }

        // What is the date the idea was created?
        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime cre_date { get; set; }

        // What is the date of the last modification?
        [Display(Name = "Modification Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime mod_date { get; set; }

        // What is the name of the individual who submitted this idea?
        [Display(Name = "Name")]
        public string cre_user { get; set; }

        // Is the idea Submitted, Accepted, Rejected, Archived, or submitted as a project?
        // Admin needs to be able to make this "Project Submission" as well. Possibly disable edit after this?
        [Required]
        [Display(Name = "Status")]
        public string statusCode { get; set; }

        // Why was this idea Denied?
        // This needs to be a conditional display of only when the status is set to "Rejected"
        [DataType(DataType.MultilineText)]
        [Display(Name = "Reason for Decision")]
        public string denialReason { get; set; }

        public virtual List<Attachment> attachments { get; set; }
    }
}