using Microsoft.EntityFrameworkCore;
using Wine.Domain.Contracts;
using Wine.Infra.Persistence;

namespace Wine.Infra.Repositories;

public class WineRepository : IWineRepository
{
    private readonly ApplicationDbContext _context;

    public WineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Domain.Entities.Wine>> GetAllAsync() => await _context.Wines.ToListAsync();

    public async Task<Domain.Entities.Wine> GetByIdAsync(int id) => await _context.Wines.FindAsync(id);

    public async Task<Domain.Entities.Wine> AddAsync(Domain.Entities.Wine wine)
    {
        await _context.Wines.AddAsync(wine);
        await _context.SaveChangesAsync();
        return wine;
    }

    public async Task<Domain.Entities.Wine> UpdateAsync(Domain.Entities.Wine wine)
    {
        _context.Wines.Update(wine);
        await _context.SaveChangesAsync();
        return wine;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deletedCount = await _context.Wines.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}