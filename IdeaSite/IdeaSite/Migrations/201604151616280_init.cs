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
                        cre_user = c.String(),
                        body = c.String(nullable: false),
                        cre_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ideaID = c.Int(nullable: false),
                        cre_date = c.DateTime(nullable: false),
                        storageLocation = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        body = c.String(nullable: false),
                        cre_date = c.DateTime(nullable: false),
                        cre_user = c.String(),
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
