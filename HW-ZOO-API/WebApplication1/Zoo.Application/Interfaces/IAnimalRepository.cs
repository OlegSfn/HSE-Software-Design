using Zoo.Domain.Entities;

namespace Zoo.Application.Interfaces
{
    public interface IAnimalRepository
    {
        Task<Animal?> GetByIdAsync(Guid id);
        Task<IEnumerable<Animal>> GetAllAsync();
        Task<IEnumerable<Animal>> GetByEnclosureIdAsync(Guid enclosureId);
        Task AddAsync(Animal animal);
        Task UpdateAsync(Animal animal);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}