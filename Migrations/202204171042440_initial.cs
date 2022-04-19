namespace Documents_backend.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentDataItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Field = c.Int(nullable: false),
                        Row = c.Int(),
                        Col = c.Int(),
                        Document_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Document_Id)
                .Index(t => new { t.Field, t.Row, t.Col }, unique: true, name: "IX_ItemPlacement")
                .Index(t => t.Document_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 500),
                        Type = c.Int(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
                        UpdateDate = c.DateTime(nullable: false, storeType: "date",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "getutcdate()")
                                },
                            }),
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
                        Permissions = c.Byte(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "31")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Signs",
                c => new
                    {
                        DocumentId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Signed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.DocumentId, t.UserId })
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DocumentId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 300),
                        UpdateDate = c.DateTime(storeType: "date",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "getutcdate()")
                                },
                            }),
                        Depricated = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
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
                        Restriction = c.String(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "")
                                },
                            }),
                        RestrictionType = c.Int(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
                        Required = c.Boolean(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
                        DataType = c.Int(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
                        TemplateTableId = c.Int(),
                        Rows = c.Int(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "1")
                                },
                            }),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .ForeignKey("dbo.TemplateItems", t => t.TemplateTableId)
                .Index(t => t.TemplateId)
                .Index(t => t.TemplateTableId);
            
            CreateTable(
                "dbo.TemplateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentDataItems", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.Templates", "TemplateTypeId", "dbo.TemplateTypes");
            DropForeignKey("dbo.TemplateItems", "TemplateTableId", "dbo.TemplateItems");
            DropForeignKey("dbo.TemplateItems", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Documents", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Templates", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Signs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Signs", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "AuthorId", "dbo.Users");
            DropIndex("dbo.TemplateItems", new[] { "TemplateTableId" });
            DropIndex("dbo.TemplateItems", new[] { "TemplateId" });
            DropIndex("dbo.Templates", new[] { "AuthorId" });
            DropIndex("dbo.Templates", new[] { "TemplateTypeId" });
            DropIndex("dbo.Signs", new[] { "UserId" });
            DropIndex("dbo.Signs", new[] { "DocumentId" });
            DropIndex("dbo.Documents", new[] { "AuthorId" });
            DropIndex("dbo.Documents", new[] { "TemplateId" });
            DropIndex("dbo.DocumentDataItems", new[] { "Document_Id" });
            DropIndex("dbo.DocumentDataItems", "IX_ItemPlacement");
            DropTable("dbo.TemplateTypes");
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
            DropTable("dbo.Signs");
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
                        "Type",
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
            DropTable("dbo.DocumentDataItems");
        }
    }
}
