using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;

namespace Zoo.Infrastructure.Repositories
{
    public class InMemoryFeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly List<FeedingSchedule> _schedules = new();

        public Task<IEnumerable<FeedingSchedule>> GetAllAsync()
        {
            return Task.FromResult(_schedules.AsEnumerable());
        }

        public Task<FeedingSchedule?> GetByIdAsync(Guid id)
        {
            var schedule = _schedules.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(schedule);
        }

        public Task<IEnumerable<FeedingSchedule>> GetByAnimalIdAsync(Guid animalId)
        {
            var schedules = _schedules.Where(s => s.AnimalId == animalId);
            return Task.FromResult(schedules);
        }

        public Task<IEnumerable<FeedingSchedule>> GetDueSchedulesAsync()
        {
            var now = DateTime.Now;
            
            var dueSchedules = _schedules.Where(s => 
                !s.IsCompleted && s.FeedingTime <= now);
                
            return Task.FromResult(dueSchedules);
        }

        public Task<IEnumerable<FeedingSchedule>> GetSchedulesForDateAsync(DateTime date)
        {
            var schedules = _schedules.Where(s => s.FeedingTime.Date == date.Date);
            return Task.FromResult(schedules);
        }

        public Task AddAsync(FeedingSchedule schedule)
        {
            if (_schedules.Any(s => s.Id == schedule.Id))
            {
                throw new InvalidOperationException($"Feeding schedule with ID {schedule.Id} already exists");
            }
            
            _schedules.Add(schedule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(FeedingSchedule schedule)
        {
            var existingIndex = _schedules.FindIndex(s => s.Id == schedule.Id);
            if (existingIndex == -1)
            {
                throw new InvalidOperationException($"Feeding schedule with ID {schedule.Id} not found");
            }
            
            _schedules[existingIndex] = schedule;
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var schedule = _schedules.FirstOrDefault(s => s.Id == id);
            if (schedule == null)
            {
                throw new InvalidOperationException($"Feeding schedule with ID {id} not found");
            }
            
            _schedules.Remove(schedule);
            return Task.FromResult(true);
        }
        
        public Task<bool> RemoveAsync(Guid id)
        {
            return DeleteAsync(id);
        }
        
        public Task<bool> ExistsAsync(Guid id)
        {
            return Task.FromResult(_schedules.Any(s => s.Id == id));
        }
    }
}