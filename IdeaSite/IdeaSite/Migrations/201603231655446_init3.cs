namespace IdeaSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "ownerIdea_ideaID", "dbo.Ideas");
            DropIndex("dbo.Comments", new[] { "ownerIdea_ideaID" });
            DropColumn("dbo.Comments", "ownerName");
            DropColumn("dbo.Comments", "ownerDescription");
            DropColumn("dbo.Comments", "ownerIdea_ideaID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "ownerIdea_ideaID", c => c.Int());
            AddColumn("dbo.Comments", "ownerDescription", c => c.String());
            AddColumn("dbo.Comments", "ownerName", c => c.String());
            CreateIndex("dbo.Comments", "ownerIdea_ideaID");
            AddForeignKey("dbo.Comments", "ownerIdea_ideaID", "dbo.Ideas", "ideaID");
        }
    }
}
