using AppPlusSQL.Domain.Entities;

namespace AppPlusSQL.Application.Interfaces
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();
        Task<IEnumerable<object>> GetLastActivitiesPerUserAsync();
        Task<Activity?> GetActivityByIdAsync(int id);
        Task<IEnumerable<Activity>> GetActivitiesByMemberIdAsync(int memberId);
    }
}
