using AppPlusSQL.Domain.Entities;

namespace AppPlusSQL.Application.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task<Member?> GetMemberByIdAsync(int id);
        Task<IEnumerable<Member>> GetMembersByTeamIdAsync(int teamId);
    }
}
