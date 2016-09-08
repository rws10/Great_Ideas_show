using System.Data.Entity;
using log4net;
using IdeaSite.Models;

    public class Great_Ideas : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public Great_Ideas() : base("Great_Ideas")
        {
            Database.SetInitializer<Great_Ideas>(null);
        }

    //Server[@Name='agadm049423\sqlexpress']/Database[@Name='Grea_Ideas']/Table[@Name='Mails' and @Schema='dbo']

    public DbSet<Idea> Ideas { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Mail> Mails { get; set; }
    }
