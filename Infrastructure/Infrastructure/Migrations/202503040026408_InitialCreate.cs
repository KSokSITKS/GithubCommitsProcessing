namespace Persistence.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Commits",
                c => new
                    {
                        Sha = c.String(nullable: false, maxLength: 128),
                        RepositoryId = c.Guid(nullable: false),
                        Message = c.String(),
                        Committer = c.String(),
                    })
                .PrimaryKey(t => t.Sha);
            
            CreateTable(
                "dbo.Repoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Owner = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Repoes");
            DropTable("dbo.Commits");
        }
    }
}
