namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite74 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemInstance", "HP1");
            DropColumn("dbo.ItemInstance", "MP1");
            DropColumn("dbo.ItemInstance", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemInstance", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ItemInstance", "MP1", c => c.Short());
            AddColumn("dbo.ItemInstance", "HP1", c => c.Short());
        }
    }
}
