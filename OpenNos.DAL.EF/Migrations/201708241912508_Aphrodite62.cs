namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite62 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.ItemInstance", "ShellRarity");

        public override void Up() => AddColumn("dbo.ItemInstance", "ShellRarity", c => c.Byte());

        #endregion
    }
}