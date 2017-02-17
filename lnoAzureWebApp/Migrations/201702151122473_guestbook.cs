namespace lnoAzureWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guestbook : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GuestBookEntries",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        GuestName = c.String(),
                        PhotoUrl = c.String(),
                        ThumbUrl = c.String(),
                        CreateDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GuestBookEntries");
        }
    }
}
