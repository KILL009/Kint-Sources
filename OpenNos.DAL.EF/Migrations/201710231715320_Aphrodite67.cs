namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite67 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.ShellEffect", "ItemInstance_Id", c => c.Guid());
            CreateIndex("dbo.ShellEffect", "ItemInstance_Id");
            AddForeignKey("dbo.ShellEffect", "ItemInstance_Id", "dbo.ItemInstance", "Id");
        }

        public override void Up()
        {
            DropForeignKey("dbo.ShellEffect", "ItemInstance_Id", "dbo.ItemInstance");
            DropForeignKey("dbo.ShellEffect", "ItemInstanceId", "dbo.ItemInstance");
            DropIndex("dbo.ShellEffect", new[] { "ItemInstance_Id" });
            DropColumn("dbo.ShellEffect", "ItemInstance_Id");
        }

        #endregion
    }
}