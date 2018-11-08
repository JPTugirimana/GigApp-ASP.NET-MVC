namespace GigAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttendance1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Attendances", new[] { "Gig_Id" });
            AlterColumn("dbo.Attendances", "Gig_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Attendances", "Gig_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Attendances", new[] { "Gig_Id" });
            AlterColumn("dbo.Attendances", "Gig_Id", c => c.Int());
            CreateIndex("dbo.Attendances", "Gig_Id");
        }
    }
}
