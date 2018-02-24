namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite63 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.NpcMonster", "Catch");

        public override void Up() => AddColumn("dbo.NpcMonster", "Catch", c => c.Boolean(nullable: false));

        #endregion
    }
}