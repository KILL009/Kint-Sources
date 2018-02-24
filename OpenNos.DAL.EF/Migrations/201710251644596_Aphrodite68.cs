namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite68 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            CreateTable(
                "dbo.CellonOption",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Level = c.Byte(nullable: false),
                    Type = c.Byte(nullable: false),
                    Value = c.Int(nullable: false),
                    EquipmentSerialId = c.Guid(nullable: false),
                    WearableInstance_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id);

            CreateIndex("dbo.CellonOption", "WearableInstance_Id");
            AddForeignKey("dbo.CellonOption", "WearableInstance_Id", "dbo.ItemInstance", "Id");
        }

        public override void Up()
        {
            DropForeignKey("dbo.CellonOption", "WearableInstance_Id", "dbo.ItemInstance");
            DropIndex("dbo.CellonOption", new[] { "WearableInstance_Id" });
            DropTable("dbo.CellonOption");
        }

        #endregion
    }
}