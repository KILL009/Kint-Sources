namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite68 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.MapType", "MapTypeId", c => c.Short(nullable: false));
        }

        public override void Up() => DropColumn("dbo.MapType", "MapTypeId");

        #endregion
    }
}