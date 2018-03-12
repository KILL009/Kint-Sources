namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite64 : DbMigration
    {
        #region Methods

        public override void Down() => AddColumn("dbo.ItemInstance", "CellonOptionId", c => c.Guid());

        public override void Up() => DropColumn("dbo.ItemInstance", "CellonOptionId");

        #endregion
    }
}