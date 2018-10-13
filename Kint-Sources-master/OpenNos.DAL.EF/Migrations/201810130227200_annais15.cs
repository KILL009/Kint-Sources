namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mate", "PrestigeLevl", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mate", "PrestigeLevl");
        }
    }
}
