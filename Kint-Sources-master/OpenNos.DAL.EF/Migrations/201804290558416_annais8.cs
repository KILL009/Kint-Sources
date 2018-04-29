namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annais8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MapMonster", "Position", c => c.Int(nullable: false));
            AlterColumn("dbo.MapNpc", "Position", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MapNpc", "Position", c => c.Byte(nullable: false));
            AlterColumn("dbo.MapMonster", "Position", c => c.Byte(nullable: false));
        }
    }
}
