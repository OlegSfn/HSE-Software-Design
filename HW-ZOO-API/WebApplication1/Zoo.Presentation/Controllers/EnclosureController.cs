using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Presentation.DTOs;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnclosureController : ControllerBase
    {
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly IAnimalRepository _animalRepository;
        
        public EnclosureController(
            IEnclosureRepository enclosureRepository,
            IAnimalRepository animalRepository)
        {
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all enclosures in the zoo",
            Description = "Retrieves a collection of all enclosures currently in the zoo",
            OperationId = "GetAllEnclosures",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnclosureDTO>))]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of all enclosures was successfully retrieved", typeof(IEnumerable<EnclosureDTO>))]
        public async Task<ActionResult<IEnumerable<EnclosureDTO>>> GetAllEnclosures()
        {
            var enclosures = await _enclosureRepository.GetAllAsync();
            var enclosureDtos = enclosures.Select(MapToDTO);
            
            return Ok(enclosureDtos);
        }
        
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Gets a specific enclosure by ID",
            Description = "Retrieves information about an enclosure by its ID",
            OperationId = "GetEnclosureById",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnclosureDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The enclosure was found and returned successfully", typeof(EnclosureDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No enclosure with the specified ID was found")]
        public async Task<ActionResult<EnclosureDTO>> GetEnclosureById(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
                
            return Ok(MapToDTO(enclosure));
        }
        
        [HttpGet("available")]
        [SwaggerOperation(
            Summary = "Gets all available enclosures",
            Description = "Retrieves enclosures that have space available for more animals",
            OperationId = "GetAvailableEnclosures",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnclosureDTO>))]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of available enclosures was successfully retrieved", typeof(IEnumerable<EnclosureDTO>))]
        public async Task<ActionResult<IEnumerable<EnclosureDTO>>> GetAvailableEnclosures()
        {
            var enclosures = await _enclosureRepository.GetAvailableEnclosuresAsync();
            var enclosureDtos = enclosures.Select(MapToDTO);
            
            return Ok(enclosureDtos);
        }
        
        [HttpGet("type/{type}")]
        [SwaggerOperation(
            Summary = "Gets enclosures by type",
            Description = "Retrieves all enclosures of a specific type (Predator, Herbivore, Bird, Aquatic)",
            OperationId = "GetEnclosuresByType",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnclosureDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of enclosures was successfully retrieved", typeof(IEnumerable<EnclosureDTO>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid enclosure type specified")]
        public async Task<ActionResult<IEnumerable<EnclosureDTO>>> GetEnclosuresByType(string type)
        {
            try
            {
                var enclosures = await _enclosureRepository.GetByTypeAsync(type);
                var enclosureDtos = enclosures.Select(MapToDTO);
                
                return Ok(enclosureDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new enclosure",
            Description = "Adds a new enclosure to the zoo with specified type, size, and capacity",
            OperationId = "CreateEnclosure",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EnclosureDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status201Created, "The enclosure was successfully created", typeof(EnclosureDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
        public async Task<ActionResult<EnclosureDTO>> CreateEnclosure(CreateEnclosureDTO createEnclosureDto)
        {
            try
            {
                var enclosureType = EnclosureType.FromString(createEnclosureDto.Type);
                var enclosureSize = NonNegativeInt.FromInt(createEnclosureDto.Size);
                var enclosureCapacity = NonNegativeInt.FromInt(createEnclosureDto.MaxCapacity);
                if (enclosureSize < enclosureCapacity)
                {
                    throw new ArgumentException("Enclosure size must be greater than or equal to capacity", nameof(createEnclosureDto.Size));
                }
                
                var enclosure = new Enclosure(
                    enclosureType,
                    enclosureSize,
                    enclosureCapacity);
                
                await _enclosureRepository.AddAsync(enclosure);
                
                return CreatedAtAction(nameof(GetEnclosureById), new { id = enclosure.Id }, MapToDTO(enclosure));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Deletes an enclosure",
            Description = "Removes an empty enclosure from the zoo",
            OperationId = "DeleteEnclosure",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The enclosure was successfully deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Cannot delete enclosure with animals in it")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No enclosure with the specified ID was found")]
        public async Task<IActionResult> DeleteEnclosure(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            if (enclosure.CurrentAnimalCount > NonNegativeInt.FromInt(0))
            {
                return BadRequest("Cannot delete enclosure with animals in it. Remove all animals first.");
            }
                
            await _enclosureRepository.RemoveAsync(id);
            return NoContent();
        }
        
        [HttpPut("{id:guid}/clean")]
        [SwaggerOperation(
            Summary = "Cleans an enclosure",
            Description = "Updates the cleaning timestamp for the specified enclosure",
            OperationId = "CleanEnclosure",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnclosureDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The enclosure was successfully cleaned", typeof(EnclosureDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No enclosure with the specified ID was found")]
        public async Task<ActionResult<EnclosureDTO>> CleanEnclosure(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
                
            enclosure.Clean();
            await _enclosureRepository.UpdateAsync(enclosure);
            
            return Ok(MapToDTO(enclosure));
        }
        
        [HttpGet("{id:guid}/animals")]
        [SwaggerOperation(
            Summary = "Gets animals in an enclosure",
            Description = "Retrieves all animals that are currently in the specified enclosure",
            OperationId = "GetAnimalsInEnclosure",
            Tags = new[] { "Enclosures" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimalDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of animals in the enclosure was successfully retrieved", typeof(IEnumerable<AnimalDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No enclosure with the specified ID was found")]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimalsInEnclosure(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
                
            var animals = await _animalRepository.GetByEnclosureIdAsync(id);
            var animalDtos = animals.Select(a => new AnimalDTO
            {
                Id = a.Id,
                AnimalType = a.Type.ToString(),
                Name = a.Name.ToString(),
                DateOfBirth = a.DateOfBirth,
                Gender = a.Gender.ToString(),
                FavoriteFood = a.FavoriteFood.ToString(),
                HealthStatus = a.HealthStatus.ToString(),
                EnclosureId = a.EnclosureId
            });
            
            return Ok(animalDtos);
        }
        
        private static EnclosureDTO MapToDTO(Enclosure enclosure)
        {
            return new EnclosureDTO
            {
                Id = enclosure.Id,
                Type = enclosure.Type.ToString(),
                Size = enclosure.Size.ToInt(),
                MaxCapacity = enclosure.MaxCapacity.ToInt(),
                CurrentAnimalCount = enclosure.CurrentAnimalCount.ToInt(),
                LastCleaned = enclosure.LastCleaned,
                AnimalIds = enclosure.AnimalIds
            };
        }
    }
}