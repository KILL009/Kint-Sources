namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite54 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.BCard", "IsLevelScaled");

        public override void Up() => AddColumn("dbo.BCard", "IsLevelScaled", c => c.Boolean(nullable: false));

        #endregion
    }
}