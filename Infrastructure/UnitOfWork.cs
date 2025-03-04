using Domain;
using Persistence.Database;

namespace Persistence
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}
	}
}
