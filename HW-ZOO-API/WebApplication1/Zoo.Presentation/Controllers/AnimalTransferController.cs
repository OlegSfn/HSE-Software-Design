using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zoo.Application.Services;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalTransferController : ControllerBase
    {
        private readonly AnimalTransferService _transferService;
        
        public AnimalTransferController(AnimalTransferService transferService)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
        }
        
        [HttpPost("transfer")]
        [SwaggerOperation(
            Summary = "Transfers an animal to a new enclosure",
            Description = "Moves an animal from its current enclosure to a another one, only if the new is appropriate",
            OperationId = "TransferAnimal",
            Tags = new[] { "Animal Transfers" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, "Animal was successfully transferred to the target enclosure")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data or incompatible transfer attempted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal or enclosure not found")]
        public async Task<IActionResult> TransferAnimal(TransferRequest request)
        {
            if (request.AnimalId == Guid.Empty || request.TargetEnclosureId == Guid.Empty)
            {
                return BadRequest("Animal ID and Target Enclosure ID must be provided");
            }
                
            try
            {
                var result = await _transferService.TransferAnimalAsync(request.AnimalId, request.TargetEnclosureId);
                if (result)
                {
                    return Ok($"Animal {request.AnimalId} successfully transferred to enclosure {request.TargetEnclosureId}");
                }
                
                return BadRequest("Cannot transfer animal. Target enclosure may be full or incompatible with the animal.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpPost("remove")]
        [SwaggerOperation(
            Summary = "Removes an animal from its current enclosure",
            Description = "Takes an animal out of its current enclosure without placing it in a new one",
            OperationId = "RemoveAnimalFromEnclosure",
            Tags = new[] { "Animal Transfers" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Animal was successfully removed from its enclosure")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data or animal not in an enclosure")]
        public async Task<IActionResult> RemoveAnimalFromEnclosure(RemoveFromEnclosureRequest request)
        {
            if (request.AnimalId == Guid.Empty)
            {
                return BadRequest("Animal ID must be provided");
            }

            try
            {
                var result = await _transferService.RemoveAnimalFromEnclosureAsync(request.AnimalId);
                if (!result)
                {
                    return BadRequest("Animal is not assigned to any enclosure");
                }

                return Ok($"Animal {request.AnimalId} successfully removed from its enclosure");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
    /// <summary>
    /// Request model for transferring an animal to a new enclosure
    /// </summary>
    public class TransferRequest
    {
        /// <summary>
        /// Unique identifier of the animal to transfer
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid AnimalId { get; set; }
        
        /// <summary>
        /// Unique identifier of the target enclosure
        /// </summary>
        /// <example>7bfde1f0-5943-4c5d-9e8d-25c5d5d1bfcd</example>
        public Guid TargetEnclosureId { get; set; }
    }
    
    /// <summary>
    /// Request model for removing an animal from its current enclosure
    /// </summary>
    public class RemoveFromEnclosureRequest
    {
        /// <summary>
        /// Unique identifier of the animal to remove from its enclosure
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid AnimalId { get; set; }
    }
}