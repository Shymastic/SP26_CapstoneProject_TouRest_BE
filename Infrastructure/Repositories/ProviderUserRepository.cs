using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderUserRepository : BaseRepository<ProviderUser>, IProviderUserRepository
    {
        public ProviderUserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AddUserIntoProvider(Guid providerId, Guid userId, ProviderUserRole role)
        {
            var providerUser = new ProviderUser { ProviderId = providerId, UserId = userId, Role = role };
            await _context.ProviderUsers.AddAsync(providerUser);
            await _context.SaveChangesAsync();
        }

        public async Task<ProviderUser?> GetByUserIdAsync(Guid userId)
        {
            return await _context.ProviderUsers.Include(x=>x.Provider).Include(x=>x.User)
                .FirstOrDefaultAsync(pu => pu.UserId == userId);
        }
        
    }
}
