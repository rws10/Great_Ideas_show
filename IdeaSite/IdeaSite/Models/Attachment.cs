using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.IO;
using log4net;

namespace IdeaSite.Models
{
    public class Attachment : IEnumerable
    {
        [Key]
        [Required]
        public int ID { get; set; }

        public int IdeaID { get; set; }

        public virtual Idea ownIdea { get; set; }

        public string name { get; set; }

        public DateTime cre_date { get; set; }

        [Required]
        [StringLength(255)]
        public string storageLocation { get; set; }

        public bool delete { get; set; }

        public void DeleteFile()
        {
            if (Directory.Exists(storageLocation))
            {
                var file = String.Format("{0}\\{1}", storageLocation, name);

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        public void DeleteDirectory()
        {
            if (Directory.Exists(storageLocation))
            {
                if (!Directory.EnumerateFiles(storageLocation).Any())
                {
                    Directory.Delete(storageLocation, true);
                }
            }
        }


        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)storageLocation).GetEnumerator();
        }
    }
}