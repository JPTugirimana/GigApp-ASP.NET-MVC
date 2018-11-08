namespace GigAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttendance2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Attendances", name: "Gig_Id", newName: "GigId");
            RenameIndex(table: "dbo.Attendances", name: "IX_Gig_Id", newName: "IX_GigId");
            DropPrimaryKey("dbo.Attendances");
            AddPrimaryKey("dbo.Attendances", new[] { "GigId", "AttendeeId" });
            DropColumn("dbo.Attendances", "GidId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attendances", "GidId", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.Attendances");
            AddPrimaryKey("dbo.Attendances", new[] { "GidId", "AttendeeId" });
            RenameIndex(table: "dbo.Attendances", name: "IX_GigId", newName: "IX_Gig_Id");
            RenameColumn(table: "dbo.Attendances", name: "GigId", newName: "Gig_Id");
        }
    }
}
