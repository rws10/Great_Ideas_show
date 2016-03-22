namespace IdeaSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ownerName", c => c.String());
            AddColumn("dbo.Comments", "ownerDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "ownerDescription");
            DropColumn("dbo.Comments", "ownerName");
        }
    }
}
