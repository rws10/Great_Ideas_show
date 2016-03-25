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
                        userName = c.String(nullable: false),
                        bodyOfComment = c.String(nullable: false),
                        creationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        storageLocation = c.String(nullable: false, maxLength: 255),
                        ideaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        description = c.String(nullable: false),
                        creationDate = c.DateTime(nullable: false),
                        submitter = c.String(nullable: false),
                        statusCode = c.String(nullable: false),
                        denialReason = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ideas");
            DropTable("dbo.Files");
            DropTable("dbo.Comments");
        }
    }
}
