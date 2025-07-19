using AppPlusSQL.Persistence.Data;
using AppPlusSQL.Domain.Entities;
using AppPlusSQL.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Profiling;

namespace AppPlusSQL.Persistence.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            using (MiniProfiler.Current.Step("Pobieranie wszystkich członków"))
            {
                return await _context.Members
                    .Include(m => m.Team)
                    .Include(m => m.Activities)
                    .ToListAsync();
            }
        }

        public async Task<Member?> GetMemberByIdAsync(int id)
        {
            using (MiniProfiler.Current.Step($"Pobieranie członka o ID: {id}"))
            {
                var memeber = await _context.Members
                    .Include(m => m.Team)
                    .Include(m => m.Activities)
                    .FirstOrDefaultAsync(m => m.Id == id);
                return memeber;
            }
        }

        public async Task<IEnumerable<Member>> GetMembersByTeamIdAsync(int teamId)
        {
            using (MiniProfiler.Current.Step($"Pobieranie członków dla zespołu o ID: {teamId}"))
            {
                return await _context.Members
                    .Include(m => m.Team)
                    .Include(m => m.Activities)
                    .Where(m => m.TeamId == teamId)
                    .ToListAsync();
            }
        }
    }
}
