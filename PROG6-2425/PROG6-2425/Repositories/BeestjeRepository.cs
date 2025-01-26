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

    public IEnumerable<Beestje> GetAll()
    {
        return _dbContext.Beestjes.ToList();
    }

    public Beestje GetById(int id)
    {
        return _dbContext.Beestjes.Find(id);
    }
    
    public IEnumerable<Beestje> GetBeestjesByIds(List<int> beestjeIds)
    {
        if (beestjeIds == null || !beestjeIds.Any())
        {
            return new List<Beestje>();
        }

        return _dbContext.Beestjes
            .Where(beestje => beestjeIds.Contains(beestje.BeestjeId))
            .ToList();
    }

    public void Create(Beestje beestje)
    {
        _dbContext.Beestjes.Add(beestje);
        _dbContext.SaveChanges();
    }

    public void Update(Beestje beestje)
    {
        _dbContext.Beestjes.Update(beestje);
        _dbContext.SaveChanges();
    }

    public void Delete(int id)
    {
        var beestje = _dbContext.Beestjes.Find(id);
        if (beestje != null)
        {
            _dbContext.Beestjes.Remove(beestje);
            _dbContext.SaveChanges();
        }
    }
}