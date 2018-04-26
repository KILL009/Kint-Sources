namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais6 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ItemInstance", "Amount", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ItemInstance", "Amount", c => c.Int(nullable: false));
        }
    }
}
