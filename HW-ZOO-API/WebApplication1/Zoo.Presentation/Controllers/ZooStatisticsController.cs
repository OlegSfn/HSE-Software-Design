using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zoo.Application.Services;
using Zoo.Presentation.DTOs;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZooStatisticsController : ControllerBase
    {
        private readonly ZooStatisticsService _statisticsService;
        
        public ZooStatisticsController(ZooStatisticsService statisticsService)
        {
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        }
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves zoo statistics",
            Description = "Retrieves statistics about the zoo including animal counts, enclosure usage, and feeding data",
            OperationId = "GetZooStatistics",
            Tags = new[] { "Statistics" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ZooStatisticsDTO))]
        [SwaggerResponse(StatusCodes.Status200OK, "Zoo statistics were successfully retrieved", typeof(ZooStatisticsDTO))]
        public async Task<ActionResult<ZooStatisticsDTO>> GetZooStatistics()
        {
            var statistics = await _statisticsService.GetZooStatisticsAsync();
            
            var statisticsDto = new ZooStatisticsDTO
            {
                TotalAnimals = statistics.TotalAnimals,
                HealthyAnimals = statistics.HealthyAnimals,
                SickAnimals = statistics.SickAnimals,
                TotalEnclosures = statistics.TotalEnclosures,
                AvailableEnclosures = statistics.AvailableEnclosures,
                FullEnclosures = statistics.FullEnclosures,
                EnclosuresByType = statistics.EnclosuresByType,
                AnimalsBySpecies = statistics.AnimalsBySpecies,
                PendingFeedings = statistics.PendingFeedings,
                CompletedFeedings = statistics.CompletedFeedings,
                FeedingsToday = statistics.FeedingsToday,
                GenderDistribution = statistics.GenderDistribution
            };
            
            return Ok(statisticsDto);
        }
        
        [HttpGet("daily")]
        [SwaggerOperation(
            Summary = "Retrieves daily zoo statistics",
            Description = "Retrieves general statistics and today's feedings",
            OperationId = "GetDailyZooStatistics",
            Tags = new[] { "Statistics" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ZooStatisticsDTO))]
        [SwaggerResponse(StatusCodes.Status200OK, "Daily zoo statistics were successfully retrieved", typeof(ZooStatisticsDTO))]
        public async Task<ActionResult<ZooStatisticsDTO>> GetDailyZooStatistics()
        {
            var statistics = await _statisticsService.GetZooStatisticsAsync();
            
            var statisticsDto = new ZooStatisticsDTO
            {
                TotalAnimals = statistics.TotalAnimals,
                HealthyAnimals = statistics.HealthyAnimals,
                SickAnimals = statistics.SickAnimals,
                TotalEnclosures = statistics.TotalEnclosures,
                AvailableEnclosures = statistics.AvailableEnclosures,
                FullEnclosures = statistics.FullEnclosures,
                EnclosuresByType = statistics.EnclosuresByType,
                AnimalsBySpecies = statistics.AnimalsBySpecies,
                PendingFeedings = statistics.PendingFeedings,
                CompletedFeedings = statistics.CompletedFeedings,
                FeedingsToday = statistics.FeedingsToday,
                GenderDistribution = statistics.GenderDistribution
            };
            
            return Ok(statisticsDto);
        }
    }
}