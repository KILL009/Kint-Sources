namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite65 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.Mail", "AttachmentLevel");

        public override void Up() => AddColumn("dbo.Mail", "AttachmentLevel", c => c.Byte(nullable: false));

        #endregion
    }
}