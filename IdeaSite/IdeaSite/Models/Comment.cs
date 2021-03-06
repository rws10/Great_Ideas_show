﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;


namespace IdeaSite.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public int ideaID { get; set; }

        [Display(Name = "Name")]
        public string cre_user { get; set; }

        [Required(ErrorMessage = "Comment Required")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comment")]
        public string body { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime cre_date { get; set; }
    }
}