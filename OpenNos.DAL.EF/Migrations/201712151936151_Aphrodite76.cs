namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aphrodite76 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mall",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemVnum = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Mall");
        }
    }
}
