namespace GKHNNC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        AgentID = c.Int(nullable: false),
                        Name = c.String(),
                        Password = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.AgentID);
            
            CreateTable(
                "dbo.Sopostavlenies",
                c => new
                    {
                        SopostavlenieId = c.Int(nullable: false, identity: true),
                        WorkId = c.Int(nullable: false),
                        AgentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SopostavlenieId)
                .ForeignKey("dbo.Agents", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("dbo.Works", t => t.WorkId, cascadeDelete: true)
                .Index(t => t.WorkId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "dbo.Works",
                c => new
                    {
                        WorkId = c.Int(nullable: false),
                        Name = c.String(),
                        Group = c.String(),
                        Izmerenie = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.WorkId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sopostavlenies", "WorkId", "dbo.Works");
            DropForeignKey("dbo.Sopostavlenies", "AgentId", "dbo.Agents");
            DropIndex("dbo.Sopostavlenies", new[] { "AgentId" });
            DropIndex("dbo.Sopostavlenies", new[] { "WorkId" });
            DropTable("dbo.Works");
            DropTable("dbo.Sopostavlenies");
            DropTable("dbo.Agents");
        }
    }
}
