using System;
using System.Linq;
using DatabaseMigration;
using DatabaseMigration.Model;

namespace BackOffice.Repositories
{
	public class UserRepository
	{
		private readonly DrowsinessDetectionContext _context;
		public UserRepository(DrowsinessDetectionContext context)
		{
			_context = context;
		}
		public IEnumerable<AppUser> GetUsers()
		{
			return _context.Users.ToList();
		}
	}
}

