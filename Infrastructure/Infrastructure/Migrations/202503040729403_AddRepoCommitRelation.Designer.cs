﻿// <auto-generated />
namespace Persistence.Infrastructure.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.5.1")]
    public sealed partial class AddRepoCommitRelation : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddRepoCommitRelation));
        
        string IMigrationMetadata.Id
        {
            get { return "202503040729403_AddRepoCommitRelation"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
