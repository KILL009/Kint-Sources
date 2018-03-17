namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite77 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinigameLog",
                c => new
                    {
                        MinigameLogId = c.Long(nullable: false, identity: true),
                        StartTime = c.Long(nullable: false),
                        EndTime = c.Long(nullable: false),
                        Score = c.Int(nullable: false),
                        Minigame = c.Byte(nullable: false),
                        CharacterId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.MinigameLogId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);
            
            AddColumn("dbo.Character", "LastFamilyLeave", c => c.Long(nullable: false));
            AlterColumn("dbo.Family", "MaxSize", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinigameLog", "CharacterId", "dbo.Character");
            DropIndex("dbo.MinigameLog", new[] { "CharacterId" });
            AlterColumn("dbo.Family", "MaxSize", c => c.Byte(nullable: false));
            DropColumn("dbo.Character", "LastFamilyLeave");
            DropTable("dbo.MinigameLog");
        }
    }
}
