namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite61 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropForeignKey("dbo.ShellEffect", "ItemInstanceId", "dbo.ItemInstance");
            DropIndex("dbo.ShellEffect", new[] { "ItemInstanceId" });
            DropTable("dbo.ShellEffect");
        }

        public override void Up()
        {
            CreateTable(
                "dbo.ShellEffect",
                c => new
                {
                    ShellEffectId = c.Long(nullable: false, identity: true),
                    EffectLevel = c.Byte(nullable: false),
                    Effect = c.Byte(nullable: false),
                    Value = c.Short(nullable: false),
                    ItemInstanceId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ShellEffectId)
                .ForeignKey("dbo.ItemInstance", t => t.ItemInstanceId)
                .Index(t => t.ItemInstanceId);
        }

        #endregion
    }
}