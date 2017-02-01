namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedHeartRate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "HeartRate", c => c.Int(nullable: false));
            AddColumn("dbo.Samples", "Heartrate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Samples", "Heartrate");
            DropColumn("dbo.Locations", "HeartRate");
        }
    }
}
