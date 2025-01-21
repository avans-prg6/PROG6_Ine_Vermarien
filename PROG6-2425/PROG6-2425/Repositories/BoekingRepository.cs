using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public class BoekingRepository : IBoekingRepository
{
    private readonly BeestFeestDbContext _dbContext;

    public BoekingRepository(BeestFeestDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Boeking> CreateBoekingAsync(Boeking boeking)
    {
        await _dbContext.Boekingen.AddAsync(boeking);
        await _dbContext.SaveChangesAsync();
        return boeking;
    }
    
    public async Task<IEnumerable<Beestje>> GetBeestjesByDatumAsync(DateTime datum)
    {
        // Selecteer alle beestjes die NIET in een boeking staan op de gegeven datum
        var beschikbareBeestjes = await _dbContext.Beestjes
            .Where(b => !_dbContext.Boekingen
                .Any(bo => bo.Datum == datum && bo.Beestjes
                    .Any(bk => bk.BeestjeId == b.BeestjeId)))
            .ToListAsync();

        return beschikbareBeestjes;
    }

    public async Task<IEnumerable<Beestje>> GetAllBeestjesAsync()
    {
        return await _dbContext.Beestjes.ToListAsync();
    }

    public async Task<List<KlantenKaartType>> GetKlantenKaartTypesAsync()
    {
        return await _dbContext.KlantenKaartTypes.ToListAsync();
    }

    public async Task<IEnumerable<Boeking>> GetBoekingenByUserId(string id)
    {
        List<Boeking> boekingen = await _dbContext.Boekingen
            .Where(u => u.AccountId == id)
            .ToListAsync();
        return boekingen;
    }

    public async Task<Boeking?> GetBoekingById(int id)
    {
        return await _dbContext.Boekingen.FindAsync(id);
    }

}