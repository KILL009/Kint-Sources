namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "prestigeLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "prestigeLevel");
        }
    }
}
