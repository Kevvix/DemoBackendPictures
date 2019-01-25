namespace Demo.Backend.Pictures.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Photos", "PhotoSizeFormat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "PhotoSizeFormat", c => c.Int(nullable: false));
        }
    }
}
