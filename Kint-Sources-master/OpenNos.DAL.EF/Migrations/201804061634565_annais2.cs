namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BazaarItem", "Amount", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "Design", c => c.Short(nullable: false));
            AddColumn("dbo.ItemInstance", "Upgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.Mail", "AttachmentAmount", c => c.Byte(nullable: false));
            AddColumn("dbo.Mail", "AttachmentUpgrade", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mail", "AttachmentUpgrade");
            DropColumn("dbo.Mail", "AttachmentAmount");
            DropColumn("dbo.ItemInstance", "Upgrade");
            DropColumn("dbo.ItemInstance", "Design");
            DropColumn("dbo.BazaarItem", "Amount");
        }
    }
}
