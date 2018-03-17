namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite66 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            CreateTable(
                "dbo.ShellEffectGeneration",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Effect = c.Byte(nullable: false),
                    EffectLevel = c.Byte(nullable: false),
                    MaximumValue = c.Byte(nullable: false),
                    MinimumValue = c.Byte(nullable: false),
                    Rare = c.Byte(nullable: false),
                    ShellEffectGenerationId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.ItemInstance", "CellonOptionId", c => c.Guid());
            DropForeignKey("dbo.CellonOption", "WearableInstance_Id", "dbo.ItemInstance");
            DropIndex("dbo.CellonOption", new[] { "WearableInstance_Id" });
            DropIndex("dbo.ShellEffect", new[] { "ItemInstance_Id" });
            AlterColumn("dbo.CellonOption", "WearableInstance_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.ShellEffect", "ItemInstance_Id", c => c.Guid(nullable: false));
            DropColumn("dbo.CellonOption", "EquipmentSerialId");
            DropColumn("dbo.ShellEffect", "EquipmentSerialId");
            DropColumn("dbo.ItemInstance", "EquipmentSerialId");
            RenameColumn(table: "dbo.CellonOption", name: "WearableInstance_Id", newName: "WearableInstanceId");
            RenameColumn(table: "dbo.ShellEffect", name: "ItemInstance_Id", newName: "ItemInstanceId");
            CreateIndex("dbo.CellonOption", "WearableInstanceId");
            CreateIndex("dbo.ShellEffect", "ItemInstanceId");
            AddForeignKey("dbo.CellonOption", "WearableInstanceId", "dbo.ItemInstance", "Id", cascadeDelete: true);
        }

        public override void Up()
        {
            DropForeignKey("dbo.CellonOption", "WearableInstanceId", "dbo.ItemInstance");
            DropIndex("dbo.ShellEffect", new[] { "ItemInstanceId" });
            DropIndex("dbo.CellonOption", new[] { "WearableInstanceId" });
            RenameColumn(table: "dbo.ShellEffect", name: "ItemInstanceId", newName: "ItemInstance_Id");
            RenameColumn(table: "dbo.CellonOption", name: "WearableInstanceId", newName: "WearableInstance_Id");
            AddColumn("dbo.ItemInstance", "EquipmentSerialId", c => c.Guid());
            AddColumn("dbo.ShellEffect", "EquipmentSerialId", c => c.Guid(nullable: false));
            AddColumn("dbo.CellonOption", "EquipmentSerialId", c => c.Guid(nullable: false));
            AlterColumn("dbo.ShellEffect", "ItemInstance_Id", c => c.Guid());
            AlterColumn("dbo.CellonOption", "WearableInstance_Id", c => c.Guid());
            CreateIndex("dbo.ShellEffect", "ItemInstance_Id");
            CreateIndex("dbo.CellonOption", "WearableInstance_Id");
            AddForeignKey("dbo.CellonOption", "WearableInstance_Id", "dbo.ItemInstance", "Id");
            DropColumn("dbo.ItemInstance", "CellonOptionId");
            DropTable("dbo.ShellEffectGeneration");
        }

        #endregion
    }
}