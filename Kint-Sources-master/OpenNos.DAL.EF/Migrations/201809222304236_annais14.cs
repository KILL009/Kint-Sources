namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "IsPrestige", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "IsPrestige");
        }
    }
}
