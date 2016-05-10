using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaSite.Models
{
    public class FileSelectionViewModel
    {
        public Idea idea { get; set; }
        public List<FileSelectorEditorViewModel> Attachs { get; set; }

        public FileSelectionViewModel()
        {
            this.Attachs = new List<FileSelectorEditorViewModel>();
        }


        public IEnumerable<int> getSelectedIds()
        {
            // Return an Enumerable containing the Id's of the selected people:
            return (from p in this.Attachs where p.Selected select p.ID).ToList();
        }
    }
}