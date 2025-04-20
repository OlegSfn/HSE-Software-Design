using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zoo.Application.Interfaces;
using Zoo.Application.Services;
using Zoo.Domain.ValueObjects;
using Zoo.Presentation.DTOs;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedingScheduleController : ControllerBase
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly FeedingOrganizationService _feedingService;
        
        public FeedingScheduleController(
            IFeedingScheduleRepository feedingScheduleRepository,
            IAnimalRepository animalRepository,
            FeedingOrganizationService feedingService)
        {
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _feedingService = feedingService ?? throw new ArgumentNullException(nameof(feedingService));
        }
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves all feeding schedules",
            Description = "Retrieves a collection of all feeding schedules in the zoo",
            OperationId = "GetAllSchedules",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeedingScheduleDTO>))]
        public async Task<ActionResult<IEnumerable<FeedingScheduleDTO>>> GetAllSchedules()
        {
            var schedules = await _feedingScheduleRepository.GetAllAsync();
            var scheduleDtos = await MapSchedulesToDTOsAsync(schedules);
            
            return Ok(scheduleDtos);
        }
        
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Retrieves a feeding schedule by ID",
            Description = "Retrieves a specific feeding schedule by its ID",
            OperationId = "GetScheduleById",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedingScheduleDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeedingScheduleDTO>> GetScheduleById(Guid id)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
                
            var dto = await MapScheduleToDTOAsync(schedule);
            
            return Ok(dto);
        }
        
        [HttpGet("date/{date}")]
        [SwaggerOperation(
            Summary = "Retrieves feeding schedules by date",
            Description = "Retrieves all feeding schedules for a specific date",
            OperationId = "GetSchedulesByDate",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeedingScheduleDTO>))]
        public async Task<ActionResult<IEnumerable<FeedingScheduleDTO>>> GetSchedulesByDate(DateTime date)
        {
            var schedules = await _feedingService.GetSchedulesForDateAsync(date);
            var scheduleDtos = await MapSchedulesToDTOsAsync(schedules);
            
            return Ok(scheduleDtos);
        }
        
        [HttpGet("animal/{animalId:guid}")]
        [SwaggerOperation(
            Summary = "Retrieves feeding schedules for an animal",
            Description = "Retrieves all feeding schedules for a specific animal",
            OperationId = "GetSchedulesByAnimal",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeedingScheduleDTO>))]
        public async Task<ActionResult<IEnumerable<FeedingScheduleDTO>>> GetSchedulesByAnimal(Guid animalId)
        {
            try
            {
                var schedules = await _feedingService.GetAnimalSchedulesAsync(animalId);
                var scheduleDtos = await MapSchedulesToDTOsAsync(schedules);

                return Ok(scheduleDtos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }

        }
        
        [HttpGet("due")]
        [SwaggerOperation(
            Summary = "Retrieves all due feeding schedules",
            Description = "Retrieves all feeding schedules that are due but not completed",
            OperationId = "GetDueSchedules",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeedingScheduleDTO>))]
        public async Task<ActionResult<IEnumerable<FeedingScheduleDTO>>> GetDueSchedules()
        {
            var schedules = await _feedingScheduleRepository.GetDueSchedulesAsync();
            var scheduleDtos = await MapSchedulesToDTOsAsync(schedules);
            
            return Ok(scheduleDtos);
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new feeding schedule",
            Description = "Creates a new feeding schedule to the zoo",
            OperationId = "CreateSchedule",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FeedingScheduleDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeedingScheduleDTO>> CreateSchedule(CreateFeedingScheduleDTO createDto)
        {
            try
            {
                var animal = await _animalRepository.GetByIdAsync(createDto.AnimalId);
                if (animal == null)
                {
                    return NotFound($"Animal with ID {createDto.AnimalId} not found");
                }
                
                var foodType = FoodType.FromString(createDto.FoodType);
                
                var scheduleId = await _feedingService.ScheduleFeedingAsync(
                    createDto.AnimalId,
                    createDto.FeedingTime,
                    foodType);
                
                var schedule = await _feedingScheduleRepository.GetByIdAsync(scheduleId);
                var dto = await MapScheduleToDTOAsync(schedule);
                
                return CreatedAtAction(nameof(GetScheduleById), new { id = scheduleId }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Updates a feeding schedule",
            Description = "Updates an existing feeding schedule with new information",
            OperationId = "UpdateSchedule",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedingScheduleDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeedingScheduleDTO>> UpdateSchedule(Guid id, UpdateFeedingScheduleDTO updateDto)
        {
            try
            {
                var foodType = FoodType.FromString(updateDto.FoodType);
                
                await _feedingService.UpdateFeedingScheduleAsync(
                    id,
                    updateDto.FeedingTime,
                    foodType);
                
                var schedule = await _feedingScheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    return NotFound();
                }
                
                var dto = await MapScheduleToDTOAsync(schedule);
                
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id:guid}/complete")]
        [SwaggerOperation(
            Summary = "Marks a feeding schedule as completed",
            Description = "Updates the status of a feeding schedule to completed",
            OperationId = "CompleteSchedule",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedingScheduleDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeedingScheduleDTO>> CompleteSchedule(Guid id)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            
            
            await _feedingService.MarkFeedingAsCompletedAsync(id);
            var dto = await MapScheduleToDTOAsync(schedule);
            
            return Ok(dto);
        }
        
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Deletes a feeding schedule",
            Description = "Deletes a feeding schedule from the zoo",
            OperationId = "DeleteSchedule",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            
            await _feedingService.DeleteFeedingScheduleAsync(id);
            
            return NoContent();
        }
        
        [HttpPost("check-due")]
        [SwaggerOperation(
            Summary = "Checks for due feedings",
            Description = "Checks for due feeding schedules and sends notifications",
            OperationId = "CheckDueFeedings",
            Tags = new[] { "Feeding" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckDueFeedings()
        {
            await _feedingService.CheckAndNotifyDueFeedings();
            
            return Ok("Due feedings checked and notifications sent");
        }
        
        private async Task<FeedingScheduleDTO> MapScheduleToDTOAsync(Domain.Entities.FeedingSchedule schedule)
        {
            var animal = await _animalRepository.GetByIdAsync(schedule.AnimalId);
            
            return new FeedingScheduleDTO
            {
                Id = schedule.Id,
                AnimalId = schedule.AnimalId,
                AnimalName = animal!.Name.ToString(),
                FeedingTime = schedule.FeedingTime,
                FoodType = schedule.FoodType.ToString(),
                IsCompleted = schedule.IsCompleted
            };
        }
        
        private async Task<IEnumerable<FeedingScheduleDTO>> MapSchedulesToDTOsAsync(IEnumerable<Domain.Entities.FeedingSchedule> schedules)
        {
            var result = new List<FeedingScheduleDTO>();
            
            foreach (var schedule in schedules)
            {
                result.Add(await MapScheduleToDTOAsync(schedule));
            }
            
            return result;
        }
    }
}