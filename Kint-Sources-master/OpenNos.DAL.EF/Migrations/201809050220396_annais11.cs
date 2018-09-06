namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Character", "HeroLevel", c => c.Int(nullable: false));
            AlterColumn("dbo.Character", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Character", "Level", c => c.Byte(nullable: false));
            AlterColumn("dbo.Character", "HeroLevel", c => c.Byte(nullable: false));
        }
    }
}
