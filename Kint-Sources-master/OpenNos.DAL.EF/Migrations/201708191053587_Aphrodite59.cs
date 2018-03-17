namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite59 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.Character", "Reput", c => c.Long(nullable: false));
            DropColumn("dbo.Character", "Reputation");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "Reputation", c => c.Long(nullable: false));
            DropColumn("dbo.Character", "Reput");
        }

        #endregion
    }
}