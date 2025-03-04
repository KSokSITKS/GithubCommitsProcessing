using Domain.Entities;
using System.Data.Entity;
using System.Reflection.Emit;

namespace Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext() : base("name=DefaultConnection")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Commit>()
				.HasRequired(c => c.Repository)
				.WithMany(r => r.Commits)
				.HasForeignKey(c => c.RepositoryId)
				.WillCascadeOnDelete(false);

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Repo> Repos { get; set; }
		public DbSet<Commit> Commits { get; set; }
	}
}
