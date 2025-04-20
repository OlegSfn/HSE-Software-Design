using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;

namespace Zoo.Infrastructure.Repositories
{
    public class InMemoryAnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _animals = new();

        public Task<IEnumerable<Animal>> GetAllAsync()
        {
            return Task.FromResult(_animals.AsEnumerable());
        }

        public Task<Animal?> GetByIdAsync(Guid id)
        {
            var animal = _animals.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(animal);
        }

        public Task<IEnumerable<Animal>> GetByEnclosureIdAsync(Guid enclosureId)
        {
            var animals = _animals.Where(a => a.EnclosureId == enclosureId);
            return Task.FromResult(animals);
        }

        public Task AddAsync(Animal animal)
        {
            if (_animals.Any(a => a.Id == animal.Id))
            {
                throw new InvalidOperationException($"Animal with ID {animal.Id} already exists");
            }
            
            _animals.Add(animal);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Animal animal)
        {
            var existingIndex = _animals.FindIndex(a => a.Id == animal.Id);
            if (existingIndex == -1)
            {
                throw new InvalidOperationException($"Animal with ID {animal.Id} not found");
            }
            
            _animals[existingIndex] = animal;
            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            var animal = _animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                throw new InvalidOperationException($"Animal with ID {id} not found");
            }
            
            _animals.Remove(animal);
            return Task.FromResult(true);

        }
        
        public Task<bool> ExistsAsync(Guid id)
        {
            return Task.FromResult(_animals.Any(a => a.Id == id));
        }
    }
}