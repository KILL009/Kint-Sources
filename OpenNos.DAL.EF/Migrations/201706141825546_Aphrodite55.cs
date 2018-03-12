namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite55 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.RollGeneratedItem", "OriginalItemRare", c => c.Byte(nullable: false));
        }

        public override void Up() => DropColumn("dbo.RollGeneratedItem", "OriginalItemRare");

        #endregion
    }
}