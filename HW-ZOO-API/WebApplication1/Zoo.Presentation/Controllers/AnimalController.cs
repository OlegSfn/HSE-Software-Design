using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zoo.Application.Interfaces;
using Zoo.Application.Services;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Presentation.DTOs;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly AnimalTransferService _animalTransferService;
        
        public AnimalController(
            IAnimalRepository animalRepository,
            AnimalTransferService animalTransferService)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _animalTransferService = animalTransferService ?? throw new ArgumentNullException(nameof(animalTransferService));
        }
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all animals in the zoo",
            Description = "Retrieves a collection of all animals currently in the zoo",
            OperationId = "GetAllAnimals",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimalDTO>))]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of all animals was successfully retrieved", typeof(IEnumerable<AnimalDTO>))]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAllAnimals()
        {
            var animals = await _animalRepository.GetAllAsync();
            var animalDtos = animals.Select(MapToDTO);
            
            return Ok(animalDtos);
        }
        
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Gets an animal by ID",
            Description = "Retrieves information about a specific animal by its ID",
            OperationId = "GetAnimalById",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The animal was found and returned successfully", typeof(AnimalDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No animal with the specified ID was found")]
        public async Task<ActionResult<AnimalDTO>> GetAnimalById(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
                
            return Ok(MapToDTO(animal));
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new animal",
            Description = "Adds a new animal to the zoo",
            OperationId = "CreateAnimal",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status201Created, "The animal was successfully created", typeof(AnimalDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
        public async Task<ActionResult<AnimalDTO>> CreateAnimal(CreateAnimalDTO createAnimalDto)
        {
            try
            {
                var animalType = AnimalType.FromString(createAnimalDto.AnimalType);
                var name = Name.FromString(createAnimalDto.Name);
                var gender = Gender.FromString(createAnimalDto.Gender);
                var favoriteFood = FoodType.FromString(createAnimalDto.FavoriteFood);
                
                var animal = new Animal(
                    animalType,
                    name,
                    createAnimalDto.DateOfBirth,
                    gender,
                    favoriteFood);
                
                await _animalRepository.AddAsync(animal);
                
                return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, MapToDTO(animal));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Deletes an animal",
            Description = "Removes an animal from the zoo database. If the animal is in an enclosure, it is removed from an enclosure",
            OperationId = "DeleteAnimal",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The animal was successfully deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No animal with the specified ID was found")]
        public async Task<IActionResult> DeleteAnimal(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
                
            if (animal.EnclosureId.HasValue)
            {
                await _animalTransferService.RemoveAnimalFromEnclosureAsync(id);
            }
            
            await _animalRepository.RemoveAsync(id);
            return NoContent();
        }
        
        [HttpPut("{id:guid}/health")]
        [SwaggerOperation(
            Summary = "Updates an animal's health status",
            Description = "Changes the health status of an animal (Healthy or Sick)",
            OperationId = "UpdateAnimalHealth",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The animal's health status was successfully updated", typeof(AnimalDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid health status provided")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No animal with the specified ID was found")]
        public async Task<ActionResult<AnimalDTO>> UpdateAnimalHealth(Guid id, UpdateAnimalHealthDTO updateDto)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
                
            try
            {
                var healthStatus = HealthStatus.FromString(updateDto.HealthStatus);
                
                if (Equals(healthStatus, HealthStatus.Healthy))
                    animal.Treat();
                else if (Equals(healthStatus, HealthStatus.Sick))
                    animal.MakeSick();
                    
                await _animalRepository.UpdateAsync(animal);
                
                return Ok(MapToDTO(animal));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id:guid}/feed")]
        [SwaggerOperation(
            Summary = "Feeds an animal",
            Description = "Records that an animal has been fed",
            OperationId = "FeedAnimal",
            Tags = new[] { "Animals" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "The animal was successfully fed", typeof(AnimalDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No animal with the specified ID was found")]
        public async Task<ActionResult<AnimalDTO>> FeedAnimal(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
                
            animal.Feed();
            await _animalRepository.UpdateAsync(animal);
            
            return Ok(MapToDTO(animal));
        }
        
        private static AnimalDTO MapToDTO(Animal animal)
        {
            return new AnimalDTO
            {
                Id = animal.Id,
                AnimalType = animal.Type.ToString(),
                Name = animal.Name.ToString(),
                DateOfBirth = animal.DateOfBirth,
                Gender = animal.Gender.ToString(),
                FavoriteFood = animal.FavoriteFood.ToString(),
                HealthStatus = animal.HealthStatus.ToString(),
                EnclosureId = animal.EnclosureId
            };
        }
    }
}