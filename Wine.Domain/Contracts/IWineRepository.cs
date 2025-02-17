
namespace Wine.Domain.Contracts;
public interface IWineRepository
{
    Task<IEnumerable<Entities.Wine>> GetAllAsync();
    Task<Entities.Wine> GetByIdAsync(int id);
    Task<Entities.Wine> AddAsync(Entities.Wine wine);
    Task<Entities.Wine> UpdateAsync(Entities.Wine wine);
    Task<bool> DeleteAsync(int id);
}
