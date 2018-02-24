namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite57 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.BCard", "IsLevelDivided");

        public override void Up() => AddColumn("dbo.BCard", "IsLevelDivided", c => c.Boolean(nullable: false));

        #endregion
    }
}