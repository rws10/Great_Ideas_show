namespace IdeaSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ideaID = c.Int(nullable: false),
                        ideaName = c.String(),
                        userName = c.String(),
                        bodyOfComment = c.String(),
                        creationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        creationDate = c.DateTime(nullable: false),
                        statusCode = c.String(),
                        denialReason = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ideas");
            DropTable("dbo.Comments");
        }
    }
}
