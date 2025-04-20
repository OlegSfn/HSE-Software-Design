using Zoo.Domain.Entities;

namespace Zoo.Application.Interfaces
{
    public interface IFeedingScheduleRepository
    {
        Task<FeedingSchedule?> GetByIdAsync(Guid id);
        Task<IEnumerable<FeedingSchedule>> GetAllAsync();
        Task<IEnumerable<FeedingSchedule>> GetByAnimalIdAsync(Guid animalId);
        Task<IEnumerable<FeedingSchedule>> GetDueSchedulesAsync();
        Task<IEnumerable<FeedingSchedule>> GetSchedulesForDateAsync(DateTime date);
        Task AddAsync(FeedingSchedule schedule);
        Task UpdateAsync(FeedingSchedule schedule);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}