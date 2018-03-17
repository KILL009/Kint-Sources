namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite78 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "NosheatDollar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "NosheatDollar");
        }
    }
}
