namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite62 : DbMigration
    {
        #region Methods

        public override void Down() => DropTable("dbo.ShellEffectGeneration");

        public override void Up()
        {
            CreateTable(
                "dbo.ShellEffectGeneration",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Effect = c.Byte(nullable: false),
                    EffectLevel = c.Byte(nullable: false),
                    MaximumValue = c.Byte(nullable: false),
                    MinimumValue = c.Byte(nullable: false),
                    Rare = c.Byte(nullable: false),
                    ShellEffectGenerationId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        #endregion
    }
}