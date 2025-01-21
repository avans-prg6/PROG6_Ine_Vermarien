using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public class BeestjeRepository : IBeestjeRepository
{
    private readonly BeestFeestDbContext _dbContext;

    public BeestjeRepository(BeestFeestDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Beestje>> GetAllAsync()
    {
        return await _dbContext.Beestjes.ToListAsync();
    }

    public async Task<Beestje?> GetByIdAsync(int id)
    {
        return await _dbContext.Beestjes.FindAsync(id);
    }
    
    public async Task<IEnumerable<Beestje>> GetBeestjesByIdsAsync(List<int> beestjeIds)
    {
        if (beestjeIds == null || !beestjeIds.Any())
        {
            return new List<Beestje>();
        }

        return await _dbContext.Beestjes
            .Where(beestje => beestjeIds.Contains(beestje.BeestjeId))
            .ToListAsync();
    }

    public async Task CreateAsync(Beestje beestje)
    {
        _dbContext.Beestjes.Add(beestje);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Beestje beestje)
    {
        _dbContext.Beestjes.Update(beestje);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var beestje = await _dbContext.Beestjes.FindAsync(id);
        if (beestje != null)
        {
            _dbContext.Beestjes.Remove(beestje);
            await _dbContext.SaveChangesAsync();
        }
    }
}