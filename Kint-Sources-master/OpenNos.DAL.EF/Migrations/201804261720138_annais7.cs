namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais7 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ItemInstance", "IX_SlotAndType");
            AlterColumn("dbo.ItemInstance", "Type", c => c.Short(nullable: false));
            CreateIndex("dbo.ItemInstance", new[] { "CharacterId", "Slot", "Type" }, name: "IX_SlotAndType");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ItemInstance", "IX_SlotAndType");
            AlterColumn("dbo.ItemInstance", "Type", c => c.Byte(nullable: false));
            CreateIndex("dbo.ItemInstance", new[] { "CharacterId", "Slot", "Type" }, name: "IX_SlotAndType");
        }
    }
}
