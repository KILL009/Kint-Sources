namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Annais : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BazaarItem", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BazaarItem", "Amount", c => c.Byte(nullable: false));
        }
    }
}
