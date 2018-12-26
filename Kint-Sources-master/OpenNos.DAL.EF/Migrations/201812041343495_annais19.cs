namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais19 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.NpcMonster", "prestigeLevel");
            DropColumn("dbo.NpcMonster", "PrestigeXp");
            DropColumn("dbo.Mate", "PrestigeLevl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mate", "PrestigeLevl", c => c.Byte(nullable: false));
            AddColumn("dbo.NpcMonster", "PrestigeXp", c => c.Int(nullable: false));
            AddColumn("dbo.NpcMonster", "prestigeLevel", c => c.Byte(nullable: false));
        }
    }
}
