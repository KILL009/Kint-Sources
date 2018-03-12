namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite66 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.ScriptedInstance", "Label");

        public override void Up() => AddColumn("dbo.ScriptedInstance", "Label", c => c.String());

        #endregion
    }
}