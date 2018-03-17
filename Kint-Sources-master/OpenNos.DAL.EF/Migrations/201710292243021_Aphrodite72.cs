namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite72 : DbMigration
    {
        public override void Up() => AddColumn("dbo.Character", "GoldBank", c => c.Long(nullable: false));

        public override void Down() => DropColumn("dbo.Character", "GoldBank");
    }
}
