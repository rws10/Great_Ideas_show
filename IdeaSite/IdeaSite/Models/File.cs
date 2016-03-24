using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace IdeaSite.Models
{
    public class File
    {
        [Key]
        public int ID { get; set; }

        [StringLength(255)]
        public string storageLocation { get; set; }

        public int ideaID { get; set; }
    }
}