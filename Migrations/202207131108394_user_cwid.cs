namespace Documents.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class user_cwid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Users", "ManagerId", "dbo.Users");
            DropForeignKey("dbo.Templates", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Signs", "InitiatorId", "dbo.Users");
            DropForeignKey("dbo.Signs", "UserId", "dbo.Users");
            DropIndex("dbo.Documents", new[] { "AuthorId" });
            DropIndex("dbo.Users", new[] { "ManagerId" });
            DropIndex("dbo.Templates", new[] { "AuthorId" });
            DropIndex("dbo.Signs", new[] { "UserId" });
            DropIndex("dbo.Signs", new[] { "InitiatorId" });
            DropPrimaryKey("dbo.Users");
            AddColumn("dbo.Templates", "Path", c => c.String(maxLength: 300));
            AlterColumn("dbo.Documents", "Author_CWID", c => c.String(maxLength: 8));
            AlterColumn("dbo.Users", "CWID", c => c.String(nullable: false, maxLength: 8));
            AlterColumn("dbo.Users", "Manager_CWID", c => c.String(maxLength: 8));
            AlterColumn("dbo.Templates", "Author_CWID", c => c.String(maxLength: 8));
            AddPrimaryKey("dbo.Users", "CWID");
            CreateIndex("dbo.Documents", "Author_CWID");
            CreateIndex("dbo.Users", "Manager_CWID");
            CreateIndex("dbo.Templates", "Author_CWID");
            CreateIndex("dbo.Signs", "User_CWID");
            CreateIndex("dbo.Signs", "Initiator_CWID");
            AddForeignKey("dbo.Documents", "Author_CWID", "dbo.Users", "CWID");
            AddForeignKey("dbo.Users", "Manager_CWID", "dbo.Users", "CWID");
            AddForeignKey("dbo.Templates", "Author_CWID", "dbo.Users", "CWID");
            AddForeignKey("dbo.Signs", "Initiator_CWID", "dbo.Users", "CWID");
            AddForeignKey("dbo.Signs", "User_CWID", "dbo.Users", "CWID");
        }

        public override void Down()
        {
            AddColumn("dbo.Users", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Signs", "User_CWID", "dbo.Users");
            DropForeignKey("dbo.Signs", "Initiator_CWID", "dbo.Users");
            DropForeignKey("dbo.Templates", "Author_CWID", "dbo.Users");
            DropForeignKey("dbo.Users", "Manager_CWID", "dbo.Users");
            DropForeignKey("dbo.Documents", "Author_CWID", "dbo.Users");
            DropIndex("dbo.Signs", new[] { "Initiator_CWID" });
            DropIndex("dbo.Signs", new[] { "User_CWID" });
            DropIndex("dbo.Templates", new[] { "Author_CWID" });
            DropIndex("dbo.Users", new[] { "Manager_CWID" });
            DropIndex("dbo.Documents", new[] { "Author_CWID" });
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Signs", "Initiator_CWID", c => c.Int(nullable: false));
            AlterColumn("dbo.Signs", "User_CWID", c => c.Int(nullable: false));
            AlterColumn("dbo.Templates", "Author_CWID", c => c.Int());
            AlterColumn("dbo.Users", "Manager_CWID", c => c.Int());
            AlterColumn("dbo.Users", "CWID", c => c.String(maxLength: 10));
            AlterColumn("dbo.Documents", "Author_CWID", c => c.Int());
            DropColumn("dbo.Templates", "Path");
            AddPrimaryKey("dbo.Users", "Id");
            RenameColumn(table: "dbo.Signs", name: "Initiator_CWID", newName: "InitiatorId");
            RenameColumn(table: "dbo.Templates", name: "Author_CWID", newName: "AuthorId");
            RenameColumn(table: "dbo.Signs", name: "User_CWID", newName: "UserId");
            RenameColumn(table: "dbo.Users", name: "Manager_CWID", newName: "ManagerId");
            RenameColumn(table: "dbo.Documents", name: "Author_CWID", newName: "AuthorId");
            CreateIndex("dbo.Signs", "InitiatorId");
            CreateIndex("dbo.Signs", "UserId");
            CreateIndex("dbo.Templates", "AuthorId");
            CreateIndex("dbo.Users", "ManagerId");
            CreateIndex("dbo.Documents", "AuthorId");
            AddForeignKey("dbo.Signs", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Signs", "InitiatorId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Templates", "AuthorId", "dbo.Users", "Id");
            AddForeignKey("dbo.Users", "ManagerId", "dbo.Users", "Id");
            AddForeignKey("dbo.Documents", "AuthorId", "dbo.Users", "Id");
        }
    }
}
