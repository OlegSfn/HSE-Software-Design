using Zoo.Application.Interfaces;
using Zoo.Domain.Events;

namespace Zoo.Application.Services
{
    public class AnimalTransferService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly IEventPublisher _eventPublisher;
        
        public AnimalTransferService(
            IAnimalRepository animalRepository,
            IEnclosureRepository enclosureRepository,
            IEventPublisher eventPublisher)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }
        
        public async Task<bool> TransferAnimalAsync(Guid animalId, Guid targetEnclosureId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null)
            {
                throw new ArgumentException($"Animal with ID {animalId} not found.");
            }
                
            var targetEnclosure = await _enclosureRepository.GetByIdAsync(targetEnclosureId);
            if (targetEnclosure == null)
            {
                throw new ArgumentException($"Enclosure with ID {targetEnclosureId} not found.");
            }

            if (!targetEnclosure.IsCompatibleWithAnimal(animal))
            {
                return false;
            }
                
            var previousEnclosureId = animal.EnclosureId;
            if (previousEnclosureId.HasValue)
            {
                var previousEnclosure = await _enclosureRepository.GetByIdAsync(previousEnclosureId.Value);
                if (previousEnclosure != null)
                {
                    previousEnclosure.RemoveAnimal(animalId);
                    await _enclosureRepository.UpdateAsync(previousEnclosure);
                }
                else
                {
                    throw new ArgumentException($"Previous enclosure with ID {previousEnclosureId} not found.");
                }
            }

            if (!targetEnclosure.AddAnimal(animalId))
            {
                return false;
            }
                
            animal.AssignToEnclosure(targetEnclosureId);
            
            await _animalRepository.UpdateAsync(animal);
            await _enclosureRepository.UpdateAsync(targetEnclosure);
            
            var animalMovedEvent = new AnimalMovedEvent(animalId, animal.Name, previousEnclosureId, targetEnclosureId);
            await _eventPublisher.PublishAnimalMovedEventAsync(animalMovedEvent);
            
            return true;
        }
        
        public async Task<bool> RemoveAnimalFromEnclosureAsync(Guid animalId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null)
            {
                throw new ArgumentException($"Animal with ID {animalId} not found.");
            }

            if (!animal.EnclosureId.HasValue)
            {
                return false;
            }
                
            var enclosureId = animal.EnclosureId.Value;
            var enclosure = await _enclosureRepository.GetByIdAsync(enclosureId);
            if (enclosure == null)
            {
                throw new ArgumentException($"Enclosure with ID {enclosureId} not found.");
            }

            if (enclosure.RemoveAnimal(animalId))
            {
                throw new ArgumentException($"Could not remove animal {animalId} from enclosure {enclosureId}.");
            }
            animal.RemoveFromEnclosure();
            
            await _animalRepository.UpdateAsync(animal);
            await _enclosureRepository.UpdateAsync(enclosure);
            
            return true;
        }
    }
}