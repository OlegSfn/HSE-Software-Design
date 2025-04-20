using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;

namespace Zoo.Infrastructure.Repositories
{
    public class InMemoryEnclosureRepository : IEnclosureRepository
    {
        private readonly List<Enclosure> _enclosures = new();

        public Task<IEnumerable<Enclosure>> GetAllAsync()
        {
            return Task.FromResult(_enclosures.AsEnumerable());
        }

        public Task<Enclosure?> GetByIdAsync(Guid id)
        {
            var enclosure = _enclosures.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(enclosure);
        }

        public Task<IEnumerable<Enclosure>> GetByTypeAsync(string enclosureTypeStr)
        {
            var enclosureType = EnclosureType.FromString(enclosureTypeStr);
            var enclosures = _enclosures.Where(e => e.Type.Equals(enclosureType));
            
            return Task.FromResult(enclosures);
        }

        public Task<IEnumerable<Enclosure>> GetAvailableEnclosuresAsync()
        {
            var availableEnclosures = _enclosures.Where(e => e.IsAvailable());
            return Task.FromResult(availableEnclosures);
        }

        public Task AddAsync(Enclosure enclosure)
        {
            if (_enclosures.Any(e => e.Id == enclosure.Id))
            {
                throw new InvalidOperationException($"Enclosure with ID {enclosure.Id} already exists");
            }
            
            _enclosures.Add(enclosure);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Enclosure enclosure)
        {
            var existingIndex = _enclosures.FindIndex(e => e.Id == enclosure.Id);
            if (existingIndex == -1)
            {
                throw new InvalidOperationException($"Enclosure with ID {enclosure.Id} not found");
            }
            
            _enclosures[existingIndex] = enclosure;
            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            var enclosure = _enclosures.FirstOrDefault(e => e.Id == id);
            if (enclosure == null)
            {
                throw new InvalidOperationException($"Enclosure with ID {id} not found");
            }
            
            _enclosures.Remove(enclosure);
            return Task.FromResult(true);
        }
        
        public Task<bool> ExistsAsync(Guid id)
        {
            return Task.FromResult(_enclosures.Any(e => e.Id == id));
        }
    }
}