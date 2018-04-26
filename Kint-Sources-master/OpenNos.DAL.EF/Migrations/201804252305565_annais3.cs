namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BazaarItem", "Amount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BazaarItem", "Amount", c => c.Byte(nullable: false));
        }
    }
}
