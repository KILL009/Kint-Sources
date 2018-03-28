namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemInstance", "Design");
            DropColumn("dbo.ItemInstance", "Upgrade");
            DropColumn("dbo.Mail", "AttachmentAmount");
            DropColumn("dbo.Mail", "AttachmentUpgrade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mail", "AttachmentUpgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.Mail", "AttachmentAmount", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "Upgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "Design", c => c.Short(nullable: false));
        }
    }
}
