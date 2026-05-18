using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Enums;


namespace TouRest.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Image)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    u.Username.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FullName != null && u.FullName.Contains(search)));

            query = query.OrderByDescending(u => u.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}
