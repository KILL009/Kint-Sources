namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "PrestigeXp", c => c.Long(nullable: false));
            AddColumn("dbo.NpcMonster", "prestigeLevel", c => c.Byte(nullable: false));
            AddColumn("dbo.NpcMonster", "PrestigeXp", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NpcMonster", "PrestigeXp");
            DropColumn("dbo.NpcMonster", "prestigeLevel");
            DropColumn("dbo.Character", "PrestigeXp");
        }
    }
}
