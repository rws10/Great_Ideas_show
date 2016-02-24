namespace IdeaSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        ideaID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        creationDate = c.DateTime(nullable: false),
                        statusCode = c.String(),
                        denialReason = c.String(),
                    })
                .PrimaryKey(t => t.ideaID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ideas");
        }
    }
}
