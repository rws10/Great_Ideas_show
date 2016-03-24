using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IdeaSite.Models
{
    public class IdeaSiteContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public IdeaSiteContext() : base("name=IdeaSiteContext")
        {
            //Database.SetInitializer<IdeaSiteContext>(null);
        }

        public System.Data.Entity.DbSet<IdeaSite.Models.Idea> Ideas { get; set; }

        public System.Data.Entity.DbSet<IdeaSite.Models.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<IdeaSite.Models.File> Files { get; set; }
    }
}
