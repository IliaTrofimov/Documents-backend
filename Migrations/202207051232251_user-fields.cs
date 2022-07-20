namespace Documents.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CWID", c => c.String(maxLength: 10));
            AddColumn("dbo.Users", "EmployeeType", c => c.String(maxLength: 20));
            AddColumn("dbo.Users", "LeadingSubgroup", c => c.String(maxLength: 30));
            AddColumn("dbo.Users", "ExternalCompany", c => c.String(maxLength: 30));
            AddColumn("dbo.Users", "OrgName", c => c.String(maxLength: 30));
            AddColumn("dbo.Users", "CompanyCode", c => c.String(maxLength: 30));
            AddColumn("dbo.Users", "ManagerId", c => c.Int());
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 200));
            CreateIndex("dbo.Users", "ManagerId");
            AddForeignKey("dbo.Users", "ManagerId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "ManagerId", "dbo.Users");
            DropIndex("dbo.Users", new[] { "ManagerId" });
            AlterColumn("dbo.Users", "Email", c => c.String());
            DropColumn("dbo.Users", "ManagerId");
            DropColumn("dbo.Users", "CompanyCode");
            DropColumn("dbo.Users", "OrgName");
            DropColumn("dbo.Users", "ExternalCompany");
            DropColumn("dbo.Users", "LeadingSubgroup");
            DropColumn("dbo.Users", "EmployeeType");
            DropColumn("dbo.Users", "CWID");
        }
    }
}
