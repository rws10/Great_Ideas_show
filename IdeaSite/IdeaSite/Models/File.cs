using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeaSite.Models
{
    public class File
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public int ideaID { get; set; }

        public DateTime cre_date { get; set; }

        [Required]
        [StringLength(255)]
        public string storageLocation { get; set; }


    }
}