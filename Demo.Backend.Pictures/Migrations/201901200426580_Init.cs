namespace Demo.Backend.Pictures.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilenameWithExtension = c.String(nullable: false),
                        MimeType = c.String(nullable: false),
                        PhotoSizeFormat = c.Int(nullable: false),
                        Content = c.Binary(),
                        StorageKey = c.Guid(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Photos");
        }
    }
}
