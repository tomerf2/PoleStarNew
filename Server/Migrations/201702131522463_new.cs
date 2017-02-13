namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupCaregivers", "LastUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.GroupCaregivers", "GroupID", c => c.String(maxLength: 128));
            AlterColumn("dbo.GroupCaregivers", "CaregiverID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Locations", "PatientID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Samples", "PatientID", c => c.String(maxLength: 128));
            CreateIndex("dbo.GroupCaregivers", "GroupID");
            CreateIndex("dbo.GroupCaregivers", "CaregiverID");
            CreateIndex("dbo.Groups", "Id");
            CreateIndex("dbo.Locations", "PatientID");
            CreateIndex("dbo.Samples", "PatientID");
            AddForeignKey("dbo.GroupCaregivers", "CaregiverID", "dbo.Caregivers", "Id");
            AddForeignKey("dbo.Groups", "Id", "dbo.Patients", "Id");
            AddForeignKey("dbo.GroupCaregivers", "GroupID", "dbo.Groups", "Id");
            AddForeignKey("dbo.Locations", "PatientID", "dbo.Patients", "Id");
            AddForeignKey("dbo.Samples", "PatientID", "dbo.Patients", "Id");
            DropColumn("dbo.Locations", "CaregiverID");
            DropColumn("dbo.Patients", "GroupID");
            DropColumn("dbo.Samples", "CaregiverID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Samples", "CaregiverID", c => c.String());
            AddColumn("dbo.Patients", "GroupID", c => c.String());
            AddColumn("dbo.Locations", "CaregiverID", c => c.String());
            DropForeignKey("dbo.Samples", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.Locations", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.GroupCaregivers", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.Groups", "Id", "dbo.Patients");
            DropForeignKey("dbo.GroupCaregivers", "CaregiverID", "dbo.Caregivers");
            DropIndex("dbo.Samples", new[] { "PatientID" });
            DropIndex("dbo.Locations", new[] { "PatientID" });
            DropIndex("dbo.Groups", new[] { "Id" });
            DropIndex("dbo.GroupCaregivers", new[] { "CaregiverID" });
            DropIndex("dbo.GroupCaregivers", new[] { "GroupID" });
            AlterColumn("dbo.Samples", "PatientID", c => c.String());
            AlterColumn("dbo.Locations", "PatientID", c => c.String());
            AlterColumn("dbo.GroupCaregivers", "CaregiverID", c => c.String());
            AlterColumn("dbo.GroupCaregivers", "GroupID", c => c.String());
            DropColumn("dbo.GroupCaregivers", "LastUpdated");
        }
    }
}
