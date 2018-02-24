namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite60 : DbMigration
    {
        #region Methods

        public override void Down() => AlterColumn("dbo.Character", "Faction", c => c.Int(nullable: false));

        public override void Up() => AlterColumn("dbo.Character", "Faction", c => c.Byte(nullable: false));

        #endregion
    }
}