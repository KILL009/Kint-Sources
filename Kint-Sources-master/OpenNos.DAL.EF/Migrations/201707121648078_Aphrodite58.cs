namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite58 : DbMigration
    {
        #region Methods

        public override void Down() => DropTable("dbo.MaintenanceLog");

        public override void Up()
        {
            CreateTable(
                "dbo.MaintenanceLog",
                c => new
                {
                    LogId = c.Long(nullable: false, identity: true),
                    DateEnd = c.DateTime(nullable: false),
                    DateStart = c.DateTime(nullable: false),
                    Reason = c.String(maxLength: 255),
                })
                .PrimaryKey(t => t.LogId);
        }

        #endregion
    }
}