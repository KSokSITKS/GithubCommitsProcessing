using Domain;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Database;
using Persistence.Repositories;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
		{
			var assembly = typeof(DependencyInjection).Assembly;

			services.AddSingleton<ICommitRepository, CommitRepository>();
			services.AddSingleton<IRepoRepository, RepoRepository>();
			services.AddScoped<ApplicationDbContext>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
