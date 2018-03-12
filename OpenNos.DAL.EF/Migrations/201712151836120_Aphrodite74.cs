namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite74 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "NosDollar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "NosDollar");
        }
    }
}
