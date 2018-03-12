namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite73 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "BankGold", c => c.Long(nullable: false));
            DropColumn("dbo.Character", "BankGold");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Character", "BankGold", c => c.Long(nullable: false));
            DropColumn("dbo.Account", "BankGold");
        }
    }
}
