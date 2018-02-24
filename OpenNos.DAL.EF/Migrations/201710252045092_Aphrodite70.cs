namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite70 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.CellonOption", "Id", c => c.Guid(nullable: false));
            DropPrimaryKey("dbo.CellonOption");
            DropColumn("dbo.CellonOption", "CellonOptionId");
            AddPrimaryKey("dbo.CellonOption", "Id");
        }

        public override void Up()
        {
            DropPrimaryKey("dbo.CellonOption");
            AddColumn("dbo.CellonOption", "CellonOptionId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.CellonOption", "CellonOptionId");
            DropColumn("dbo.CellonOption", "Id");
        }

        #endregion
    }
}