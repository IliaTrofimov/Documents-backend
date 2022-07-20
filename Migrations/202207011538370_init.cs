namespace Documents.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentDataItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        FieldId = c.Int(nullable: false),
                        Row = c.Int(),
                        Document_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Document_Id, cascadeDelete: true)
                .ForeignKey("dbo.TemplateItems", t => t.FieldId, cascadeDelete: true)
                .Index(t => t.FieldId)
                .Index(t => t.Document_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Type = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false, storeType: "date", defaultValueSql: "getutcdate()"),
                        ExpireDate = c.DateTime(storeType: "date"),
                        TemplateId = c.Int(nullable: false),
                        AuthorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Firstname = c.String(maxLength: 100),
                        Lastname = c.String(maxLength: 100),
                        Fathersname = c.String(maxLength: 100),
                        Permissions = c.Byte(nullable: false, defaultValue: 31),
                        PositionId = c.Int(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Position", t => t.PositionId, cascadeDelete: true)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.Position",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemplateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 300),
                        UpdateDate = c.DateTime(storeType: "date", defaultValueSql: "getutcdate()"),
                        Depricated = c.Boolean(nullable: false, defaultValue: false),
                        TemplateTypeId = c.Int(),
                        AuthorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.TemplateTypes", t => t.TemplateTypeId)
                .Index(t => t.TemplateTypeId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.TemplateItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 500),
                        TemplateId = c.Int(nullable: false),
                        Restriction = c.String(defaultValue: ""),
                        RestrictionType = c.Int(defaultValue: 0),
                        Required = c.Boolean(defaultValue: false),
                        DataType = c.Int(defaultValue: 0),
                        TemplateTableId = c.Int(),
                        Rows = c.Int(defaultValue: 1),
                        IsTable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.TemplateId)
                .ForeignKey("dbo.TemplateItems", t => t.TemplateTableId)
                .Index(t => t.TemplateId)
                .Index(t => t.TemplateTableId);
            
            CreateTable(
                "dbo.Signs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        UserId = c.Int(nullable: true),
                        Signed = c.Boolean(),
                        SignerPositionId = c.Int(nullable: false),
                        InitiatorId = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false, defaultValueSql: "getutcdate()"),
                        CreateDate = c.DateTime(nullable: false, defaultValueSql: "getutcdate()"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.InitiatorId)
                .ForeignKey("dbo.Position", t => t.SignerPositionId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DocumentId)
                .Index(t => t.UserId)
                .Index(t => t.SignerPositionId)
                .Index(t => t.InitiatorId);
            
            CreateTable(
                "dbo.TemplateTypePositions",
                c => new
                    {
                        TemplateType_Id = c.Int(nullable: false),
                        Position_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TemplateType_Id, t.Position_Id })
                .ForeignKey("dbo.TemplateTypes", t => t.TemplateType_Id, cascadeDelete: true)
                .ForeignKey("dbo.Position", t => t.Position_Id, cascadeDelete: true)
                .Index(t => t.TemplateType_Id)
                .Index(t => t.Position_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentDataItems", "FieldId", "dbo.TemplateItems");
            DropForeignKey("dbo.DocumentDataItems", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.Signs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Signs", "SignerPositionId", "dbo.Position");
            DropForeignKey("dbo.Signs", "InitiatorId", "dbo.Users");
            DropForeignKey("dbo.Signs", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Users", "PositionId", "dbo.Position");
            DropForeignKey("dbo.Templates", "TemplateTypeId", "dbo.TemplateTypes");
            DropForeignKey("dbo.TemplateItems", "TemplateTableId", "dbo.TemplateItems");
            DropForeignKey("dbo.TemplateItems", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Documents", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Templates", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.TemplateTypePositions", "Position_Id", "dbo.Position");
            DropForeignKey("dbo.TemplateTypePositions", "TemplateType_Id", "dbo.TemplateTypes");
            DropForeignKey("dbo.Documents", "AuthorId", "dbo.Users");
            DropIndex("dbo.TemplateTypePositions", new[] { "Position_Id" });
            DropIndex("dbo.TemplateTypePositions", new[] { "TemplateType_Id" });
            DropIndex("dbo.Signs", new[] { "InitiatorId" });
            DropIndex("dbo.Signs", new[] { "SignerPositionId" });
            DropIndex("dbo.Signs", new[] { "UserId" });
            DropIndex("dbo.Signs", new[] { "DocumentId" });
            DropIndex("dbo.TemplateItems", new[] { "TemplateTableId" });
            DropIndex("dbo.TemplateItems", new[] { "TemplateId" });
            DropIndex("dbo.Templates", new[] { "AuthorId" });
            DropIndex("dbo.Templates", new[] { "TemplateTypeId" });
            DropIndex("dbo.Users", new[] { "PositionId" });
            DropIndex("dbo.Documents", new[] { "AuthorId" });
            DropIndex("dbo.Documents", new[] { "TemplateId" });
            DropIndex("dbo.DocumentDataItems", new[] { "Document_Id" });
            DropIndex("dbo.DocumentDataItems", new[] { "FieldId" });
            DropTable("dbo.TemplateTypePositions");
            DropTable("dbo.Signs");
            DropTable("dbo.TemplateItems",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "DataType",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "0" },
                        }
                    },
                    {
                        "Required",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "0" },
                        }
                    },
                    {
                        "Restriction",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "" },
                        }
                    },
                    {
                        "RestrictionType",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "0" },
                        }
                    },
                    {
                        "Rows",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "1" },
                        }
                    },
                });
            DropTable("dbo.Templates",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "Depricated",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "0" },
                        }
                    },
                    {
                        "UpdateDate",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "getutcdate()" },
                        }
                    },
                });
            DropTable("dbo.TemplateTypes");
            DropTable("dbo.Position");
            DropTable("dbo.Users",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "Permissions",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "31" },
                        }
                    },
                });
            DropTable("dbo.Documents",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "UpdateDate",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "getutcdate()" },
                        }
                    },
                });
            DropTable("dbo.DocumentDataItems");
        }
    }
}
