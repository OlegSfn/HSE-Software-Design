using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.Events;
using Zoo.Domain.ValueObjects;

namespace Zoo.Application.Services
{
    public class FeedingOrganizationService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IEventPublisher _eventPublisher;
        
        public FeedingOrganizationService(
            IAnimalRepository animalRepository,
            IFeedingScheduleRepository feedingScheduleRepository,
            IEventPublisher eventPublisher)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }
        
        public async Task<Guid> ScheduleFeedingAsync(Guid animalId, DateTime feedingTime, FoodType foodType)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null)
            {
                throw new ArgumentException($"Animal with ID {animalId} not found.");
            }
                
            var feedingSchedule = new FeedingSchedule(animalId, feedingTime, foodType);
            await _feedingScheduleRepository.AddAsync(feedingSchedule);
            
            return feedingSchedule.Id;
        }
        
        public async Task<bool> UpdateFeedingScheduleAsync(Guid scheduleId, DateTime feedingTime, FoodType foodType)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new ArgumentException($"Feeding schedule with ID {scheduleId} not found.");
            }
            
            schedule.UpdateSchedule(feedingTime, foodType);
            await _feedingScheduleRepository.UpdateAsync(schedule);
            
            return true;
        }
        
        public async Task<IEnumerable<FeedingSchedule>> GetAnimalSchedulesAsync(Guid animalId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null)
            {
                throw new ArgumentException($"Animal with ID {animalId} not found.");
            }
                
            return await _feedingScheduleRepository.GetByAnimalIdAsync(animalId);
        }
        
        public async Task<IEnumerable<FeedingSchedule>> GetSchedulesForDateAsync(DateTime date)
        {
            return await _feedingScheduleRepository.GetSchedulesForDateAsync(date);
        }
        
        public async Task<IEnumerable<FeedingSchedule>> GetDueSchedulesAsync()
        {
            return await _feedingScheduleRepository.GetDueSchedulesAsync();
        }
        
        public async Task<bool> MarkFeedingAsCompletedAsync(Guid scheduleId)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new ArgumentException($"Feeding schedule with ID {scheduleId} not found.");
            }
                
            var animal = await _animalRepository.GetByIdAsync(schedule.AnimalId);
            if (animal == null)
            {
                throw new ArgumentException($"Animal with ID {schedule.AnimalId} not found.");
            }
                
            animal.Feed();
            schedule.MarkAsCompleted();
            
            await _animalRepository.UpdateAsync(animal);
            await _feedingScheduleRepository.UpdateAsync(schedule);
            
            var feedingTimeEvent = new FeedingTimeEvent(animal.Id, animal.Name, schedule.FoodType);
            await _eventPublisher.PublishFeedingTimeEventAsync(feedingTimeEvent);
            
            return true;
        }
        
        public async Task CheckAndNotifyDueFeedings()
        {
            var dueSchedules = await GetDueSchedulesAsync();
            
            foreach (var schedule in dueSchedules)
            {
                if (schedule.IsCompleted)
                {
                    continue;
                }
                
                var animal = await _animalRepository.GetByIdAsync(schedule.AnimalId);
                if (animal == null)
                {
                    continue;
                }
                
                var feedingTimeEvent = new FeedingTimeEvent(animal.Id, animal.Name, schedule.FoodType);
                await _eventPublisher.PublishFeedingTimeEventAsync(feedingTimeEvent);
            }
        }
        
        public async Task<bool> DeleteFeedingScheduleAsync(Guid scheduleId)
        {
            var isScheduleExists = await _feedingScheduleRepository.ExistsAsync(scheduleId);
            if (!isScheduleExists)
            {
                throw new ArgumentException($"Feeding schedule with ID {scheduleId} not found.");
            }
                
            return await _feedingScheduleRepository.DeleteAsync(scheduleId);
        }
    }
}