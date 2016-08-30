using System.Data.Entity;
using log4net;

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
    
        public IdeaSiteContext() : base("name=agadm049423.Great_Ideas.dbo")
        {
            //Database.SetInitializer<IdeaSiteContext>(null);
        }

        public DbSet<Idea> Ideas { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Mail> Mails { get; set; }
    }
}
