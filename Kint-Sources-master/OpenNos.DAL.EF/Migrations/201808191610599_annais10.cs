namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Mail", "AttachmentAmount", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Mail", "AttachmentAmount", c => c.Byte(nullable: false));
        }
    }
}
