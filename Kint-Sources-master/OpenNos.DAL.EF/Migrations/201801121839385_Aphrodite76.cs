namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite76 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "BankGold", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "BankGold");
        }
    }
}
