using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace IdeaSite.Models
{
    public class Comment
    {
        // change to commit correctly.
        [Key]
        public int ideaID { get; set; }
 
        public string ideaName { get; set; }

        [Display(Name = "Name")]
        public string userName { get; set; }

        [Display(Name = "Comment")]
        public string bodyOfComment { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string creationDate { get; set; }

        public int ownerID { get; set; }

        //public string loginID { get; set; }


    }
}