using Zoo.Application.Interfaces;
using Zoo.Domain.ValueObjects;

namespace Zoo.Application.Services
{
    public class ZooStatisticsService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;

        public ZooStatisticsService(
            IAnimalRepository animalRepository,
            IEnclosureRepository enclosureRepository,
            IFeedingScheduleRepository feedingScheduleRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
        }

        public async Task<ZooStatistics> GetZooStatisticsAsync()
        {
            var animals = await _animalRepository.GetAllAsync();
            var enclosures = await _enclosureRepository.GetAllAsync();
            var feedingSchedules = await _feedingScheduleRepository.GetAllAsync();

            var totalAnimals = animals.Count();
            var healthyAnimals = animals.Count(a => a.HealthStatus == HealthStatus.Healthy);
            var sickAnimals = animals.Count(a => a.HealthStatus == HealthStatus.Sick);
            
            var totalEnclosures = enclosures.Count();
            var availableEnclosures = enclosures.Count(e => e.IsAvailable());
            var fullEnclosures = enclosures.Count(e => !e.IsAvailable());
            
            var enclosuresByType = enclosures
                .GroupBy(e => e.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            var animalsBySpecies = animals
                .GroupBy(a => a.Type.Name)
                .ToDictionary(g => g.Key, g => g.Count());
            
            var pendingFeedings = feedingSchedules.Count(f => !f.IsCompleted);
            var completedFeedings = feedingSchedules.Count(f => f.IsCompleted);
            
            var feedingsToday = feedingSchedules
                .Count(f => f.FeedingTime.Date == DateTime.Today);

            var genderDistribution = animals
                .GroupBy(a => a.Gender.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            return new ZooStatistics
            {
                TotalAnimals = totalAnimals,
                HealthyAnimals = healthyAnimals,
                SickAnimals = sickAnimals,
                TotalEnclosures = totalEnclosures,
                AvailableEnclosures = availableEnclosures,
                FullEnclosures = fullEnclosures,
                EnclosuresByType = enclosuresByType,
                AnimalsBySpecies = animalsBySpecies,
                PendingFeedings = pendingFeedings,
                CompletedFeedings = completedFeedings,
                FeedingsToday = feedingsToday,
                GenderDistribution = genderDistribution
            };
        }
    }

    public class ZooStatistics
    {
        public int TotalAnimals { get; set; }
        public int HealthyAnimals { get; set; }
        public int SickAnimals { get; set; }
        public int TotalEnclosures { get; set; }
        public int AvailableEnclosures { get; set; }
        public int FullEnclosures { get; set; }
        public Dictionary<string, int> EnclosuresByType { get; set; } = new();
        public Dictionary<string, int> AnimalsBySpecies { get; set; } = new();
        public int PendingFeedings { get; set; }
        public int CompletedFeedings { get; set; }
        public int FeedingsToday { get; set; }
        public Dictionary<string, int> GenderDistribution { get; set; } = new();
    }
}