namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite64 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.Account", "ReferrerId");

        public override void Up() => AddColumn("dbo.Account", "ReferrerId", c => c.Long(nullable: false));

        #endregion
    }
}