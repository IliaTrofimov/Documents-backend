namespace Documents.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "PositionId", "dbo.Position");
            DropIndex("dbo.Users", new[] { "PositionId" });
            AlterColumn("dbo.Users", "PositionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "PositionId");
            AddForeignKey("dbo.Users", "PositionId", "dbo.Position", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "PositionId", "dbo.Position");
            DropIndex("dbo.Users", new[] { "PositionId" });
            AlterColumn("dbo.Users", "PositionId", c => c.Int());
            CreateIndex("dbo.Users", "PositionId");
            AddForeignKey("dbo.Users", "PositionId", "dbo.Position", "Id");
        }
    }
}
