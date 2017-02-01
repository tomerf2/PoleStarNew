namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedGroup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "Code", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Groups", "Code", c => c.Int(nullable: false));
        }
    }
}
