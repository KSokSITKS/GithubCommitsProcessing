namespace Persistence.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRepoCommitRelation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Commits", "RepositoryId");
            AddForeignKey("dbo.Commits", "RepositoryId", "dbo.Repoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Commits", "RepositoryId", "dbo.Repoes");
            DropIndex("dbo.Commits", new[] { "RepositoryId" });
        }
    }
}
