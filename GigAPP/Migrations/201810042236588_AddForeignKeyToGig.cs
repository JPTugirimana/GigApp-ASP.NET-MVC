namespace GigAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyToGig : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Gigs", new[] { "Artist_Id" });
            DropColumn("dbo.Gigs", "ArtistId");
            RenameColumn(table: "dbo.Gigs", name: "Artist_Id", newName: "ArtistId");
            AlterColumn("dbo.Gigs", "ArtistId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Gigs", "ArtistId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Gigs", new[] { "ArtistId" });
            AlterColumn("dbo.Gigs", "ArtistId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Gigs", name: "ArtistId", newName: "Artist_Id");
            AddColumn("dbo.Gigs", "ArtistId", c => c.Int(nullable: false));
            CreateIndex("dbo.Gigs", "Artist_Id");
        }
    }
}
