using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.DTOs;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{

    public class AgencyUserRepository : BaseRepository<AgencyUser>, IAgencyUserRepository
    {
        public AgencyUserRepository(AppDbContext context) : base(context) { }
        public async Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId)
        {
            return await _context.AgencyUsers.AnyAsync(au => au.UserId == userId && au.AgencyId == agencyId);
        }
        public async Task AddUserToAgencyAsync(Guid agencyId, Guid userId, AgencyUserRole role)
        {
            if (!await IsUserInAgencyAsync(agencyId, userId))
            {
                var agencyUser = new AgencyUser { AgencyId = agencyId, UserId = userId, Role = role };
                await _context.AgencyUsers.AddAsync(agencyUser);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId)
        {
            var agencyUser = await _context.AgencyUsers
                .FirstOrDefaultAsync(au => au.UserId == userId && au.AgencyId == agencyId);
            if (agencyUser != null)
            {
                _context.AgencyUsers.Remove(agencyUser);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<AgencyUser>> GetAgencyUsers(Guid agencyId)
        {
            return await _context.AgencyUsers.Include(au => au.User).Include(au => au.Agency)
                .Where(au => au.AgencyId == agencyId)
                .ToListAsync();
        }
        public async Task<List<AgencyUser>> GetTourGuidesByAgencyIdAsync(Guid agencyId)
        {
            return await _context.AgencyUsers
                .Include(au => au.User)
                .Where(au => au.AgencyId == agencyId && au.Role == AgencyUserRole.TourGuide)
                .ToListAsync();
        }

        public async Task<List<AgencyUser>> SearchUsersByAgency(Guid id, SearchUserByAgency search)
        {
            return await _context.AgencyUsers.Include(au => au.User)
                .Where(au => au.AgencyId == id && (string.IsNullOrEmpty(search.FullName) || au.User.FullName.ToLower().Contains(search.FullName)))
                .ToListAsync();
        }
        public async Task<AgencyUser?> GetAgencyUserByUserId(Guid userId)
        {
            return await _context.AgencyUsers.Include(au => au.User)
                .FirstOrDefaultAsync(au => au.UserId == userId);
        }
        public async Task<List<AgencyGuideDTO>> GetGuidesByAgencyIdAsync(Guid agencyId)
        {
            var now = DateTime.UtcNow;
            return await _context.AgencyUsers
                .Include(au => au.User)
                .Where(au => au.AgencyId == agencyId && au.Role == AgencyUserRole.TourGuide)
                .AsNoTracking()
                .Select(au => new AgencyGuideDTO
                {
                    UserId = au.UserId,
                    FullName = au.User.FullName ?? au.User.Username,
                    Phone = au.User.Phone,
                    Avatar = au.User.UserAvatar,
                    ActiveTours = _context.ItinerarySchedules
                        .Count(s => s.GuideId == au.UserId
                            && s.StartTime >= now
                            && s.Status == ItineraryScheduleStatus.Confirmed),
                    CompletedTotal = _context.ItinerarySchedules
                        .Count(s => s.GuideId == au.UserId
                            && s.Status == ItineraryScheduleStatus.Completed)
                })
                .ToListAsync();
        }

    }
}
