namespace IdeaSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ownerIdea_ideaID", c => c.Int());
            CreateIndex("dbo.Comments", "ownerIdea_ideaID");
            AddForeignKey("dbo.Comments", "ownerIdea_ideaID", "dbo.Ideas", "ideaID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ownerIdea_ideaID", "dbo.Ideas");
            DropIndex("dbo.Comments", new[] { "ownerIdea_ideaID" });
            DropColumn("dbo.Comments", "ownerIdea_ideaID");
        }
    }
}
