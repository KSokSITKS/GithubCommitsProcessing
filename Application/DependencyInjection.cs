using Application.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddScoped<RepoService>();

			return services;
		}
	}
}
