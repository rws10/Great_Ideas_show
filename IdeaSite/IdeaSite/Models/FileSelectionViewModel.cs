using System.Collections.Generic;
using System.Linq;

namespace IdeaSite.Models
{
    public class FileSelectionViewModel
    {
        public Idea idea { get; set; }
        public List<SelectFileEditorViewModel> Attachs { get; set; }

        public FileSelectionViewModel()
        {
            this.Attachs = new List<SelectFileEditorViewModel>();
        }


        public IEnumerable<int> getSelectedIds()
        {
            // Return an Enumerable containing the Id's of the selected people:
            return (from p in this.Attachs where p.Selected select p.ID).ToList();
        }
    }
}