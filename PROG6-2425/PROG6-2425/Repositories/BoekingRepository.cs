using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PROG6_2425.Data;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Repositories;

public class BoekingRepository : IBoekingRepository
{
    private readonly BeestFeestDbContext _dbContext;

    public BoekingRepository(BeestFeestDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Synchroniseer CreateBoeking
    public void CreateBoeking(Boeking boeking)
    {
        _dbContext.Boekingen.Add(boeking);
        _dbContext.SaveChanges();
    }

    public IEnumerable<Beestje> GetBeestjesByDatum(DateTime datum)
    {
        // Selecteer alle beestjes die NIET in een boeking staan op de gegeven datum
        var beschikbareBeestjes = _dbContext.Beestjes
            .Where(b => !_dbContext.Boekingen
                .Any(bo => bo.Datum == datum && bo.Beestjes
                    .Any(bk => bk.BeestjeId == b.BeestjeId)))
            .ToList();

        return beschikbareBeestjes;
    }

    public List<KlantenKaartType> GetKlantenKaartTypes()
    {
        return _dbContext.KlantenKaartTypes.ToList();
    }

    public List<Boeking> GetBoekingenByUserId(string id)
    {
        List<Boeking> boekingen = _dbContext.Boekingen
            .Include(b => b.Beestjes)
            .ThenInclude(bb => bb.Beestje)
            .Where(u => u.AccountId == id)
            .ToList();

        return boekingen;
    }

    public void Delete(int boekingId)
    {
        var boeking = _dbContext.Boekingen.Find(boekingId);

        if (boeking != null)
        {
            _dbContext.Boekingen.Remove(boeking);
            _dbContext.SaveChanges();
        }
    }

    public Boeking GetBoekingById(int id)
    {
        Boeking boeking = _dbContext.Boekingen
            .Include(b => b.Beestjes) 
            .ThenInclude(bb => bb.Beestje) 
            .FirstOrDefault(b => b.BoekingId == id);
        return boeking;
    }
}