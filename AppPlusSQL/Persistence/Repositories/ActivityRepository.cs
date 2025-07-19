using AppPlusSQL.Persistence.Data;
using AppPlusSQL.Domain.Entities;
using AppPlusSQL.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Profiling;

namespace AppPlusSQL.Persistence.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly AppDbContext _context;

        public ActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
        {
            using (MiniProfiler.Current.Step("Pobieranie wszystkich aktywności"))
            {
                return await _context.Activities
                    .Include(a => a.Member)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<object>> GetLastActivitiesPerUserAsync()
        {
            using (MiniProfiler.Current.Step("Pobieranie ostatnich aktywności użytkowników"))
            {
                return await _context.Members
                    .Select(member => new
                    {
                        MemberId = member.Id,
                        MemberName = member.Name,
                        LastActivity = member.Activities
                            .OrderByDescending(a => a.CreatedAt)
                            .FirstOrDefault()
                    })
                    .Where(x => x.LastActivity != null)
                    .ToListAsync();
            }
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            using (MiniProfiler.Current.Step($"Pobieranie aktywności o ID: {id}"))
            {
                return await _context.Activities
                    .Include(a => a.Member)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            using (MiniProfiler.Current.Step($"Pobieranie aktywności dla członka o ID: {memberId}"))
            {
                return await _context.Activities
                    .Include(a => a.Member)
                    .Where(a => a.MemberId == memberId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
        }
    }
}
